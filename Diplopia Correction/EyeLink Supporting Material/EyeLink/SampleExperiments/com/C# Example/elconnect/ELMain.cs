using System;

namespace elconnect
{
	class ELmain
	{
        [STAThread]
		static void Main(string[] args)
		{
            EyelinkWindow elW = new EyelinkWindow();
            elW.Show();
            try
            {
                SREYELINKLib.EL_EYE eye = SREYELINKLib.EL_EYE.EL_EYE_NONE;
                double st;
                SREYELINKLib.EyeLinkUtil elutil = new SREYELINKLib.EyeLinkUtil();
                SREYELINKLib.EyeLink el = new SREYELINKLib.EyeLink();
                

                el.open("100.1.1.1", 0);
                el.openDataFile("abc.edf");
                el.sendCommand("link_sample_data  = LEFT,RIGHT,GAZE");
                el.sendCommand("screen_pixel_coords=0,0," + elW.Width + "," + elW.Height);
                el.sendMessage("abc");
                
                

                SREYELINKLib.ELGDICal cal = elutil.getGDICal();
                cal.setCalibrationWindow(elW.Handle.ToInt32());
                cal.enableKeyCollection(true);
                el.doTrackerSetup();
                cal.enableKeyCollection(false);

                cal.enableKeyCollection(true);
                el.doDriftCorrect((short)(elW.Width / 2),(short)(elW.Height/2), true, true);
                cal.enableKeyCollection(false);

                elW.setGazeCursor(true);
                el.startRecording(false, false, true, false);
                st = elutil.currentTime();
                while ((st + 20000) > elutil.currentTime())
                {
                    SREYELINKLib.Sample s;
                    s = el.getNewestSample();
                    if (s != null)
                    {
                        if (eye != SREYELINKLib.EL_EYE.EL_EYE_NONE)
                        {
                            if (eye == SREYELINKLib.EL_EYE.EL_BINOCULAR)
                                eye = SREYELINKLib.EL_EYE.EL_LEFT;

                            float x = s.get_gx(SREYELINKLib.EL_EYE.EL_LEFT);
                            float y = s.get_gy(SREYELINKLib.EL_EYE.EL_LEFT);
                            Console.Write(s.time);
                            Console.Write("\t");
                            Console.Write(x);
                            Console.Write("\t");
                            Console.Write(y);
                            Console.WriteLine("");// New line

                            elW.setGaze((int)x, (int)y);
                        }
                        else 
                        {
                            eye = (SREYELINKLib.EL_EYE)el.eyeAvailable();
                        }
                    }
                }
                el.stopRecording();
                el.close();
                el = null;
                elutil = null;
            }
            catch (Exception)
            {
                
                throw;
            }
		}
	}
}
