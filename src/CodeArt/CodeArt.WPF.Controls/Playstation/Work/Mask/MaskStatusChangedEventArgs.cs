using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls.Playstation
{
    public sealed class MaskStatusChangedEventArgs
    {
        public MaskStatus Status { get; private set; }

        public MaskStatusChangedEventArgs(MaskStatus status)
        {
            this.Status = status;
        }
    }

    public enum MaskStatus:byte
    {
        PreOpen,
        Opened,
        PreClose,
        Closed
    }


}
