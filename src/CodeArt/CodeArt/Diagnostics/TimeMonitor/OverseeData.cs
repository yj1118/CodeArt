using System;
using System.Collections.Generic;
using System.Linq;


namespace CodeArt.Diagnostics
{
    public sealed class OverseeData
    {
        private List<OverseeTime> _times;

        internal OverseeData()
        {
        }

        internal void AddTime(OverseeTime time)
        {
            if (_times == null) _times = new List<OverseeTime>();
            _times.Add(time);
        }

        public OverseeTime GetTime(string actionName)
        {
            if (_times == null) throw new DiagnosticsException("û���ҵ�����Ϊ" + actionName + "��ʱ����Ϣ");
            OverseeTime time = _times.FirstOrDefault(item => item.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));
            if (time == null) throw new DiagnosticsException("û���ҵ�����Ϊ" + actionName + "��ʱ����Ϣ");
            return time;
        }

        public OverseeTime GetTime(int index)
        {
            if (_times == null || index < 0 || index >= _times.Count) throw new DiagnosticsException("û���ҵ����Ϊ" + index + "��ʱ����Ϣ");
            return _times[index];
        }


        public OverseeTime[] GetTimes()
        {
            if (_times == null) return new OverseeTime[] { };
            return _times.ToArray();
        }

        public void Clear()
        {
            if (_times != null)
                _times.Clear();
        }

    }
}
