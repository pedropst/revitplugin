using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Reflection;


namespace EasyEletrica
{
    public class Ribbon : IExternalApplication
    {
        const string RIBBON_TAB = "EasyElétrica";
        const string RIBBON_PANEL = "AUTOMAÇÃO";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.CreateRibbonTab(RIBBON_TAB);
            }
            catch (Exception)
            {
                //tab já existe
            }

            RibbonPanel panel = null;
            List<RibbonPanel> panels = application.GetRibbonPanels(RIBBON_TAB);
            foreach (RibbonPanel pnl in panels)
            {
                if (pnl.Name == RIBBON_PANEL)
                {
                    panel = pnl;
                    break;
                }
            }

            if (panel == null)
            {
                panel = application.CreateRibbonPanel(RIBBON_TAB, RIBBON_PANEL);
            }

            Image img = Properties.Resources.diagramButton;
            ImageSource imgSrc = GetImageSource(img);

            Image img1 = Properties.Resources.wiringButton;
            ImageSource imgSrc1 = GetImageSource(img1);


            //create the button data
            PushButtonData btnData = new PushButtonData("DIAGRAMA UNIFILAR", "DIAGRAMA UNIFILAR", Assembly.GetExecutingAssembly().Location, "EasyEletrica.DiagramaUnifilar")
            {
                ToolTip = "Cria um diagrama unifilar.",
                LongDescription = "Cria um diagrama unifilar completo.",
                Image = imgSrc,
                LargeImage = imgSrc
            };

            PushButtonData btnData1 = new PushButtonData("FIAÇÃO AUTOMÁTICA", "FIAÇÃO AUTOMÁTICA", Assembly.GetExecutingAssembly().Location, "EasyEletrica.Automation")
            {
                ToolTip = "Passa a fiação pelos eletrodutos.",
                LongDescription = "Passa toda a fiação dos eletrodutos.",
                Image = imgSrc1,
                LargeImage = imgSrc1
            };

            //add the button to the ribbon
            PushButton button = panel.AddItem(btnData) as PushButton;
            button.Enabled = true;
            PushButton button1 = panel.AddItem(btnData1) as PushButton;
            button1.Enabled = true;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private BitmapSource GetImageSource(Image img)
        {
            BitmapImage bmp = new BitmapImage();

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                bmp.BeginInit();

                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;

                bmp.EndInit();
            }

            return bmp;
        }

    }
}
