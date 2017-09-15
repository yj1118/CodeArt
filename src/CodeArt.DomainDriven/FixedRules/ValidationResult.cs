using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public sealed class ValidationResult
    {
        /// <summary>
        /// 是否满足规则
        /// </summary>
        public bool IsSatisfied
        {
            get
            {
                return _errors.Count == 0;
            }
        }

        private ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        #region 错误信息的集合

        public int ErrorCount
        {
            get
            {
                return _errors.Count;
            }
        }

        private List<ValidationError> _errors;

        public IEnumerable<ValidationError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public void AddError(DomainProperty property, string errorCode, string message)
        {
            var error = CreatePropertyError(property.Name, errorCode, message);
            _errors.Add(error);
        }

        /// <summary>
        /// 向验证结果中添加一个错误
        /// </summary>
        public void AddError(string propertyName, string errorCode, string message)
        {
            var error = CreatePropertyError(propertyName, errorCode, message);
            _errors.Add(error);
        }

        public void AddError(string message)
        {
            var error = CreateError(string.Empty, message);
            _errors.Add(error);
        }

        public void AddError(string errorCode, string message)
        {
            var error = CreateError(errorCode, message);
            _errors.Add(error);
        }

        public void Combine(ValidationResult result)
        {
            foreach (var error in result.Errors)
                _errors.Add(error);
        }

        /// <summary>
        /// 检测验证结果是否包含指定的错误代码
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public bool ContainsCode(string errorCode)
        {
            return this.GetError(errorCode) != null;
        }

        public ValidationError GetError(string errorCode)
        {
            return this.Errors.FirstOrDefault((item) =>
            {
                return item.ErrorCode.Equals(errorCode, StringComparison.OrdinalIgnoreCase);
            });
        }

        public void RemoveError(string errorCode)
        {
            if (_errors.Count == 0) return;
            var e = this.GetError(errorCode);
            if (e != null) _errors.Remove(e);
        }

        #endregion

        internal void Clear()
        {
            _errors.Clear();
        }


        public string Message
        {
            get
            {
                if (this.IsSatisfied) return "success";
                else
                {
                    StringBuilder message = new StringBuilder();
                    foreach (var error in this.Errors)
                    {
                        message.AppendLine(error.Message);
                    }
                    return message.ToString().Trim();
                }
            }
        }

        public static readonly ValidationResult Satisfied = new ValidationResult();

        /// <summary>
        /// 获得一个验证结果对象，该对象会与数据山下文共享生命周期
        /// </summary>
        /// <returns></returns>
        public static ValidationResult Create()
        {
            return Symbiosis.TryMark(_resultPool, () =>
            {
                return new ValidationResult();
            });
        }

        private static PropertyValidationError CreatePropertyError(string propertyName, string errorCode, string message)
        {
            var error = Symbiosis.TryMark(_propertyErrorPool, () =>
            {
                return new PropertyValidationError();
            });
            error.PropertyName = propertyName;
            error.ErrorCode = errorCode;
            error.Message = message;
            return error;
        }

        private static ValidationError CreateError(string errorCode, string message)
        {
            var error = Symbiosis.TryMark(_errorPool, () =>
            {
                return new ValidationError();
            });
            error.ErrorCode = errorCode;
            error.Message = message;
            return error;
        }

        #region 结果池

        private static Pool<ValidationResult> _resultPool = new Pool<ValidationResult>(() =>
        {
            return new ValidationResult();
        }, (result, phase) =>
        {
            result.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        private static Pool<ValidationError> _errorPool = new Pool<ValidationError>(() =>
        {
            return new ValidationError();
        }, (error, phase) =>
        {
            error.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });


        private static Pool<PropertyValidationError> _propertyErrorPool = new Pool<PropertyValidationError>(() =>
        {
            return new PropertyValidationError();
        }, (error, phase) =>
        {
            error.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        #endregion

    }
}
