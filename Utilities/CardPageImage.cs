﻿using PdfSharp.Drawing;

namespace Boxy_Core.Utilities
{
    public class CardPageImage
    {
        public XImage? FrontImage { get; set; }

        public XImage? BackImage { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

    }
}
