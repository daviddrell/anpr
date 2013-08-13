using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ApplicationDataClass
{
    
    public class FRAME_PAIR
    {
        private object singleton;
        int _S2255DeviceIndex;
        public int S2255DeviceIndex { set { lock (singleton) { _S2255DeviceIndex = value; } } get { lock (singleton) { return _S2255DeviceIndex; } } }
        public Bitmap bmp;
        public byte[] jpeg;
        public int serialNumber;
        public int portPairIndex;
        public enum STATE { EMPTY = 0, PARTIAL = 1, COMPLETE = 2 };
        STATE m_state = STATE.EMPTY;
        public STATE state { set { } get { lock (singleton) { return m_state; } } }
        public int SystemChannel;

        public FRAME_PAIR()
        {
            if (singleton == null) singleton = new object();
            Free();
        }

        public FRAME_PAIR Clone ()
        {
            FRAME_PAIR copy = new FRAME_PAIR();
            copy.bmp = this.bmp;// use the same reference, do not copy the bmp;
            copy.jpeg = this.jpeg;// use the same reference, do not copy the bmp;
            copy.m_state = this.m_state;
            copy.portPairIndex = this.portPairIndex;
            copy.serialNumber = this.serialNumber;
            copy._S2255DeviceIndex = this._S2255DeviceIndex;
            return (copy);
        }

        public void Free()
        {
            bmp = null;
            jpeg = null;
            m_state = STATE.EMPTY;
        }

        public void Add(byte[] Jpeg, int PortPairIndex, int deviceIndex, int SerialNumber, int systemChannel)
        {
            lock (singleton)
            {
                if (jpeg != null) return; // drop this frame, we are out of sync
                jpeg = Jpeg;

                _S2255DeviceIndex = deviceIndex;
                serialNumber = SerialNumber;
                portPairIndex = PortPairIndex;
                SystemChannel = systemChannel;

                if (bmp == null) m_state = STATE.PARTIAL;
                else m_state = STATE.COMPLETE;

            }
        }

        public void Add(Bitmap Bmp, int PortPairIndex, int deviceIndex, int SerialNumber, int systemChannel)
        {
            lock (singleton)
            {
                if (bmp != null) return; // drop this frame, we are out of sync
                bmp = Bmp;
                _S2255DeviceIndex = deviceIndex;
                serialNumber = SerialNumber;
                portPairIndex = PortPairIndex;
                SystemChannel = systemChannel;

                if (jpeg == null) m_state = STATE.PARTIAL;
                else m_state = STATE.COMPLETE;
            }
        }
    }

}
