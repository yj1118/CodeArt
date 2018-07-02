using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.DomainDriven
{
    public class ActionMetadata : IActionMetadata
    {
        private ActionMetadata() { }

        public ActionMetadata(ActionProcedure procedure)
            : this(procedure, null, null)
        {
        }

        public ActionMetadata(ActionProcedure procedure, PreCallActionCallback preCallActionCallback, CalledActionCallback calledActionCallback)
        {
            this.Procedure = procedure;
            this.PreCallActionCallback = preCallActionCallback;
            this.CalledActionCallback = calledActionCallback;
        }

        public ActionProcedure Procedure { get; private set; }

        public PreCallActionCallback PreCallActionCallback { get; private set; }

        public CalledActionCallback CalledActionCallback { get; private set; }

        #region 执行行为之前

        public bool IsRegisteredPreCall
        {
            get
            {
                return this.PreCall != null;
            }
        }

        internal void OnPreCall(object sender, DomainActionPreCallEventArgs e)
        {
            if (this.PreCall != null) this.PreCall(sender, e);
        }

        public event DomainActionPreCallEventHandler PreCall;

        #endregion

        #region 执行行为之后

        public bool IsRegisteredCalled
        {
            get
            {
                return this.Called != null;
            }
        }

        internal void OnCalled(object sender, DomainActionCalledEventArgs e)
        {
            if (this.Called != null) this.Called(sender, e);
        }

        public event DomainActionCalledEventHandler Called;

        #endregion

    }
}