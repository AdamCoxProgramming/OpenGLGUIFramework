using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LayoutFramework.Input
{
    class Clipboard
    {
        static public string getText()
        {
                string clipboardData = null;
                Exception threadEx = null;
                Thread staThread = new Thread(
                    delegate ()
                    {
                        try
                        {
                            clipboardData = System.Windows.Clipboard.GetText();
                        }

                        catch (Exception ex)
                        {
                            threadEx = ex;
                        }
                    });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();
                return clipboardData;
        }
    }
}
