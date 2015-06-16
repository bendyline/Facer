using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using Kendo.DataViz.UI;
using System.Runtime.CompilerServices;

namespace BL.UI.KendoControls
{

    public class QRCode : Control
    {
        private Kendo.DataViz.UI.QRCode qrCode;
        private QRCodeOptions qrCodeOptions;

        public QRCodeOptions Options
        {
            get
            {
                if (this.qrCodeOptions == null)
                {
                    this.qrCodeOptions = new QRCodeOptions();
                }

                return this.qrCodeOptions;
            }

            set
            {
                if (this.qrCode != null)
                {
                    this.qrCode.SetOptions(value);
                }

                this.qrCodeOptions = value;
            }
        }

        public String Value
        {
            get
            {
                return this.Options.Value;
            }
            set
            {
                this.Options.Value = value;

                if (this.qrCode != null)
                {
                    this.qrCode.Value(value);
                }
            }
        }

        public QRCode()
        {
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoDataViz(this);

            this.EnsureScript("kendo.dataviz.ui.QRCode", "js/kendo/kendo.dataviz.qrcode.min.js");
        }

        protected override void OnApplyTemplate()
        {
            Script.Literal("var j={0}; j.kendoQRCode({2}); {1}=j.data('kendoQRCode')", this.J, this.qrCode, this.qrCodeOptions);
        }

        public override void Dispose()
        {
            if (this.qrCode != null)
            {
                this.qrCode.Destroy();
            }

            base.Dispose();
        }
    }
}
