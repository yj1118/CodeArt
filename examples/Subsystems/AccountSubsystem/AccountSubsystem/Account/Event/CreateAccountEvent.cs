using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    [Event("CreateAccount")]
    public class CreateAccountEvent : DomainEvent
    {



        public CreateAccountEvent()
        {

        }


        public override void Raise()
        {
            
        }

        public override void Reverse()
        {
           
        }
    }
}
