using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.AppSetting;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    public abstract class CommandBase
    {
        public event CommandFailedEventHandler Failed;

        public event CommandEventHandler Succeeded;

        public event CommandEventHandler Completed;

        protected void ExecuteImpl(Action action)
        {
            bool success = false;
            try
            {
                action();
                success = true;
            }
            catch (Exception ex)
            {
                if (OnFailed(ex))
                    throw;
            }
            finally
            {
                if (success) OnSucceeded();
                OnCompleted();
            }
        }

        private bool OnFailed(Exception ex)
        {
            bool throwError = true;
            if (this.Failed != null)
            {
                var e = new CommandFailedEventArgs(ex);
                this.Failed(this, e);
                throwError = e.ThrowError;
            }
            return throwError;
        }

        private void OnSucceeded()
        {
            if (this.Succeeded != null)
                this.Succeeded(this, CommandEventArgs.Instance);
        }

        private void OnCompleted()
        {
            if (this.Completed != null)
                this.Completed(this, CommandEventArgs.Instance);
        }

    }


}
