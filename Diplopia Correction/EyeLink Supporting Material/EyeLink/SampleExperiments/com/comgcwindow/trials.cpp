/*****************************************************************************
 * Copyright (c) 1997 - 2003 by SR Research Ltd., All Rights Reserved        *
 *                                                                           *
 * This software is provided as is without warranty of any kind.  The entire *
 * risk as to the results and performance of this software is assumed by the *
 * user. SR Research Ltd. disclaims all warranties, either express or implied*
 * ,including but not limited, the implied warranties of merchantability,    *
 * fitness for a particular purpose, title and noninfringement, with respect *
 * to this software.                                                         *
 *                                                                           *
 *                                                                           *
 * For non-commercial use by Eyelink licencees only                          *
 *                                                                           *
 * Windows 95/98/NT/2000/XP sample experiment in C                           *
 * For use with Version 2.0 of EyeLink Windows API                           *
 *****************************************************************************/



#include "gcwindow.h"
#include <sdl_text_support.h>
#include <SDL/SDL_image.h>
#include <SDL_rotozoom.h>


SDL_Color target_fg ={0,0,0};
/************ TEXT FOR TRIAL ***********/

static char fg_text[] = 
"Buck did not read the newspapers, or he would have known that trouble was "
"brewing, not alone for himself, but for every tide-water dog, strong of "
"muscle and with warm, long hair, from Puget Sound to San Diego.  Because "
"men, groping in the Arctic darkness, had found a yellow metal, and because "
"steamship and transportation companies were booming the find, thousands of "
"men were rushing into the Northland.  These men wanted dogs, and the dogs "
"they wanted were heavy dogs, with strong muscles by which to toil, and furry "
"coats to protect them from the frost. \n"
"   Buck lived at a big house in the sun-kissed Santa Clara Valley.  Judge "
"Miller's place, it was called.  It stood back from the road, half hidden "
"among the trees, through which glimpses could be caught of the wide cool "
"veranda that ran around its four sides.";

static char bg_text[] = 
"Xxxx xxx xxx xxxx xxx xxxxxxxxxx, xx xx xxxxx xxxx xxxxx xxxx xxxxxxx xxx "
"xxxxxxx, xxx xxxxx xxx xxxxxxx, xxx xxx xxxxx xxxx-xxxxx xxx, xxxxxx xx "
"xxxxxx xxx xxxx xxxx, xxxx xxxx, xxxx Xxxxx Xxxxx xx Xxx Xxxxx.  Xxxxxxx "
"xxx, xxxxxxx xx xxx Xxxxxx xxxxxxxx, xxx xxxxx x xxxxxx xxxxx, xxx xxxxxxx "
"xxxxxxxxx xxx xxxxxxxxxxxxxx xxxxxxxxx xxxx xxxxxxx xxx xxxx, xxxxxxxxx xx "
"xxx xxxx xxxxxxx xxxx xxx Xxxxxxxxx.  Xxxxx xxx xxxxxx xxxx, xxx xxx xxxx "
"xxxx xxxxxx xxxx xxxxx xxxx, xxxx xxxxxx xxxxxxx xx xxxxx xx xxxx, xxx xxxxx "
"xxxxx xx xxxxxxx xxxx xxxx xxx xxxxx. \n"
"   Xxxx xxxxx xx x xxx xxxxx xx xxx xxx-xxxxxx Xxxxx Xxxxx Xxxxxx.  Xxxxx "
"Xxxxxx'x xxxxx, xx xxx xxxxxx.  Xx xxxxx xxxx xxxx xxx xxxx, xxxx xxxxxx "
"xxxxx xxx xxxxx, xxxxxxx xxxxx xxxxxxxx xxxxx xx xxxxxx xx xxx xxxx xxxx "
"xxxxxxx xxxx xxx xxxxxx xxx xxxx xxxxx.";


/********* PREPARE BITMAPS FOR TRIALS  **********/

SDL_Surface *fgbm=NULL;
SDL_Surface *bgbm=NULL;

/* 
	Create foreground and background bitmaps of text 
	If hwsurface == 1 then fgbm is hwsurface
	if hwsurface == 2 then bgbm is hwsurface.
*/

static int create_text_bitmaps(int hwsurface)
{
  
  set_margin(SCRWIDTH/20,SCRHEIGHT/20, SCRWIDTH/20, SCRHEIGHT/20); 

  get_new_font("Courier New", SCRHEIGHT/32, 0);   /* create a monospaced font */
  set_line_spacing(((double)(SCRHEIGHT)/16.0)/(double) get_font_height());
                                        /* Draw text and EyeLink graphics */
  if(fgbm)
	SDL_FreeSurface(fgbm);
  if(bgbm)
	SDL_FreeSurface(bgbm);

  fgbm = SDL_CreateRGBSurface((hwsurface == 1)?(SDL_HWSURFACE|SDL_ASYNCBLIT):SDL_SWSURFACE, 
	  window->w, window->h, dispinfo.bits, 0,0,0,0);
  bgbm = SDL_CreateRGBSurface((hwsurface == 2)?(SDL_HWSURFACE|SDL_ASYNCBLIT):SDL_SWSURFACE, 
	  window->w, window->h, dispinfo.bits, 0,0,0,0);

  SDL_FillRect(fgbm,NULL,SDL_MapRGB(fgbm->format, target_background_color.r, target_background_color.b, target_background_color.g));
  SDL_FillRect(bgbm,NULL,SDL_MapRGB(bgbm->format, target_background_color.r, target_background_color.b, target_background_color.g));
  graphic_printf(fgbm,target_foreground_color,WRAP,0,0,fg_text);
  graphic_printf(bgbm,target_foreground_color,WRAP,0,0,bg_text);
  return 0;
}

/*
Creates sdl surface from image file name.
if hwsurf is non zero, a hardware surface is requested
*/
SDL_Surface * image_file_bitmap(char *fname, int keepsize, int dx, int dy, int hwsurf)
{
  SDL_Surface *image = NULL;
  image = IMG_Load(fname);

	
  if(keepsize)
	return image;

  if(!dx || !dy) /* dx or dy is 0 - make it to full screen size */
  {
	  SDL_Surface * mainsurface = SDL_GetVideoSurface();
	  dx = mainsurface->w;
	  dy = mainsurface->h;
  }

  if(dx && dy && dx != image->w && dy != image->h)
  {
	  double zx = (double)dx/(double)image->w;
	  double zy = (double)dy/(double)image->h;
	  SDL_Surface *zoomed = zoomSurface(image,zx,zy,0);
	  if(zoomed)
	  {
		  if(hwsurf)
		  {
			  SDL_Surface *hwsurface = SDL_CreateRGBSurface(SDL_HWSURFACE|SDL_ASYNCBLIT,
				  zoomed->w, zoomed->h, dispinfo.bits,0,0,0,0);
			  if(hwsurface->flags & SDL_HWSURFACE)
			  {
				  SDL_BlitSurface(zoomed,NULL,hwsurface,NULL);
				  SDL_FreeSurface(zoomed);
				  zoomed = hwsurface;
			  }
			  else
				SDL_FreeSurface(hwsurface);
		  }
		  SDL_FreeSurface(image);
		  return zoomed;
	  }
  }

  return image;

}

/* 
	Creates blank surfaces
	If hwsurface == 1 then fgbm is hwsurface
	if hwsurface == 2 then bgbm is hwsurface.
*/
SDL_Surface * blank_bitmap(SDL_Color c, int hwsurface)
{
	SDL_Surface *surface = SDL_CreateRGBSurface((hwsurface == 1)?(SDL_HWSURFACE|SDL_ASYNCBLIT):SDL_SWSURFACE
		,window->w, window->h, dispinfo.bits,0,0,0,0);
	SDL_FillRect(surface, NULL,SDL_MapRGB(surface->format,c.r,c.g,c.b));
	return surface;
}

/*  Create foreground and background bitmaps of picture
	EyeLink graphics: blank display with box at center
	type: 0 = blank background, 1= blank fovea (mask), 2 = blurred background
*/
static int create_image_bitmaps(int type)
{ 
  USES_CONVERSION;
  clear_full_screen_window(target_background_color);
  get_new_font("Courier New", SCRHEIGHT/32, 1);   /* create a monospaced font */
  /* NOTE:  *** THE FOLLOWING TEXT SHOULD NOT APPEAR IN A REAL EXPERIMENT!!!! **** */
  graphic_printf(window, target_foreground_color, NONE, SCRWIDTH/2, SCRHEIGHT/2, "Loading image...");

  switch(type)
    {  
      case 0:     /* blank background */
		  {
			fgbm = image_file_bitmap("images/sacrmeto.jpg", 0, SCRWIDTH,SCRHEIGHT, 1);
			bgbm = blank_bitmap(target_background_color, 0);
			
			break;
		  }
      case 1:     /* blank fovea */
        fgbm = blank_bitmap(target_background_color,1);
        bgbm = image_file_bitmap("images/sacrmeto.jpg", 0, SCRWIDTH,SCRHEIGHT,0);
        break;
      case 2:     /* normal and blurred bitmaps, stretched to fit display */
        fgbm = image_file_bitmap("images/sacrmeto.jpg", 0, SCRWIDTH,SCRHEIGHT,1);
        bgbm = image_file_bitmap("images/sac_blur.jpg", 0, SCRWIDTH,SCRHEIGHT,0);
        break;
    }

  eyelink->sendCommand(A2W("clear_screen 0"));                  /* clear EyeLink display */
  char buff[200];
  sprintf(buff,"draw_box %d %d %d %d 15",          /* Box around fixation point */
           SCRWIDTH/2-16, SCRHEIGHT/2-16, SCRWIDTH/2+16, SCRHEIGHT/2+16);
  eyelink->sendCommand(A2W(buff));

  if(!fgbm || !bgbm)  /* Check that both bitmaps exist */
    {
      eyelink->sendMessage(A2W("ERROR: could not load image"));
      printf("ERROR: could not load an image file");
      if(fgbm) SDL_FreeSurface(fgbm);
      if(bgbm) SDL_FreeSurface(bgbm);
      return EL_SKIP_TRIAL;
    }
  return 0;
}


/*********** TRIAL SELECTOR **********/

#define NTRIALS 5  /* 5 trials for gaze contingent demo */

/*  FOR EACH TRIAL:
	- set title, TRIALID
	- Create bitmaps and EyeLink display graphics
	- Check for errors in creating bitmaps
	- Run the trial recording loop
	- Delete bitmaps
	- Return any error code
	
	  Given trial number, execute trials
	  Returns trial result code
*/
int do_gcwindow_trial(int num)
{
	 USES_CONVERSION;
  int i;
  
  
  eyelink->setOfflineMode();	/* Must be offline to draw to EyeLink screen */

  switch(num)
    {            /* #1: gaze-contingent text, normal in window, "xx" outside */
      case 1:
	    /* This supplies the title at the bottom of the eyetracker display  */
        eyelink->sendCommand(A2W("record_status_message 'GC TEXT (WINDOW)' "));
	    /* Always send a TRIALID message before starting to record.  */
	    /* It should contain trial condition data required for analysis. */
        eyelink->sendMessage(A2W("TRIALID GCTXTW"));
		/* TRIAL_VAR_DATA message is recorded for EyeLink Data Viewer analysis
		   It specifies the list of trial variables value for the trial 
		   This must be specified within the scope of an individual trial (i.e., after 
		   "TRIALID" and before "TRIAL_RESULT") 
		*/
		eyelink->sendMessage(A2W("!V TRIAL_VAR_DATA TEXT TEXT MASK"));
		/* IMGLOAD command is recorded for EyeLink Data Viewer analysis
		   It displays a default image on the overlay mode of the trial viewer screen. 
		   Writes the image filename + path info
		*/  

		if(create_text_bitmaps(1))
          {
            eyelink->sendMessage(A2W("ERROR: could not create bitmap"));
            return EL_SKIP_TRIAL;
          }
		
		eyelink->sendMessage(A2W("!V IMGLOAD FILL images/text.png"));
	

        i = gc_window_trial(fgbm, bgbm, SCRWIDTH/4, SCRHEIGHT/3, 0, 60000L);  /* Gaze-contingent window, normal text */
        SDL_FreeSurface(fgbm); fgbm = NULL;
        SDL_FreeSurface(bgbm); bgbm = NULL;
        return i;

      case 2:    /* #2: gaze-contingent text, "xx" in window, normal outside */
        eyelink->sendCommand(A2W("record_status_message 'GC TEXT (MASK)' "));
        eyelink->sendMessage(A2W("TRIALID GCTXTM"));
   		eyelink->sendMessage(A2W("!V TRIAL_VAR_DATA TEXT MASK TEXT"));
       
		if(create_text_bitmaps(2))
          {
            eyelink->sendMessage(A2W("ERROR: could not create bitmap"));
            return EL_SKIP_TRIAL;
          }

		eyelink->sendMessage(A2W("!V IMGLOAD FILL images/text.png"));
      

		i = gc_window_trial(bgbm, fgbm, SCRWIDTH/4, SCRHEIGHT/3, 1, 60000L);  /* Gaze-contingent window, masked text */
        SDL_FreeSurface(fgbm); fgbm = NULL;
        SDL_FreeSurface(bgbm); bgbm = NULL;
        return i;

      case 3:    /* #3: Image, normal in window, blank outside */
        eyelink->sendCommand(A2W("record_status_message 'GC IMAGE (WINDOW)' "));
        eyelink->sendMessage(A2W("TRIALID GCTXTW"));
		eyelink->sendMessage(A2W("!V TRIAL_VAR_DATA IMAGE IMAGE MASK"));
        
		if(create_image_bitmaps(0))
          {
            eyelink->sendMessage(A2W("ERROR: could not create bitmap"));
            return EL_SKIP_TRIAL;
          }
	
     		
		eyelink->sendMessage(A2W("!V IMGLOAD FILL images/sacrmeto.jpg"));
   
		i = gc_window_trial(fgbm, bgbm, SCRWIDTH/4, SCRHEIGHT/3, 0, 60000L);  /* Gaze-contingent window, normal image */
        SDL_FreeSurface(fgbm); fgbm = NULL;
        SDL_FreeSurface(bgbm); bgbm = NULL;
        return i;

      case 4:    /* #4: Image, blank in window, normal outside */
        eyelink->sendCommand(A2W("record_status_message 'GC IMAGE (MASK)' "));
        eyelink->sendMessage(A2W("TRIALID GCTXTM"));
 		eyelink->sendMessage(A2W("!V TRIAL_VAR_DATA IMAGE MASK IMAGE"));

		if(create_image_bitmaps(1))
          {
            eyelink->sendMessage(A2W("ERROR: could not create bitmap"));
            return EL_SKIP_TRIAL;
          }

		eyelink->sendMessage(A2W("!V IMGLOAD FILL images/sacrmeto.jpg"));

        
		i = gc_window_trial(fgbm, bgbm, SCRWIDTH/4, SCRHEIGHT/3, 1, 60000L);  /* Gaze-contingent window, masked image */
        SDL_FreeSurface(fgbm); fgbm = NULL;
        SDL_FreeSurface(bgbm); bgbm = NULL;
        return i;

      case 5:    /* #5: Image, blurred outside window */
        eyelink->sendCommand(A2W("record_status_message 'GC IMAGE (BLURRED)' "));
        eyelink->sendMessage(A2W("TRIALID GCTXTB"));
		eyelink->sendMessage(A2W("!V TRIAL_VAR_DATA IMAGE IMAGE BLURRED"));
		
		if(create_image_bitmaps(2))
          {
            eyelink->sendMessage(A2W("ERROR: could not create bitmap"));
            return EL_SKIP_TRIAL;
          }
		
		eyelink->sendMessage(A2W("!V IMGLOAD FILL images/sacrmeto.jpg"));

		i = gc_window_trial(fgbm, bgbm, SCRWIDTH/4, SCRHEIGHT/3, 0, 60000L);  /* Gaze-contingent window, masked image */
        SDL_FreeSurface(fgbm); fgbm = NULL;
        SDL_FreeSurface(bgbm); bgbm = NULL;
        return i;
    }
  return EL_ABORT_EXPT;  /* illegal trial number  */
}


/*********** TRIAL LOOP **************/


	/* This code sequences trials within a block */
	/* It calls run_trial() to execute a trial, */
	/* then interperts result code. */
	/* It places a result message in the EDF file */
	/* This example allows trials to be repeated */
	/* from the tracker ABORT menu. */
int run_trials(void)
{
  USES_CONVERSION;
  int i;
  int trial;
                 /* INITIAL CALIBRATION: matches following trials */
  SETCOLOR(target_foreground_color ,0,0,0);         /* color of calibration target */
  SETCOLOR(target_background_color,200,200,200);   /* background for drift correction */
  graphics->setForeground(target_foreground_color.r,target_foreground_color.g,target_foreground_color.b);
  graphics->setBackground(target_background_color.r,target_background_color.g,target_background_color.b);


  if(SCRWIDTH!=800 || SCRHEIGHT!=600)
    printf("Display mode is not 800x600, resizing will slow loading.");

   
  /* TRIAL_VAR_LABELS message is recorded for EyeLink Data Viewer analysis
     It specifies the list of trial variables for the trial 
     This should be written once only and put before the recording of individual trials
  */
  eyelink->sendMessage(A2W("TRIAL_VAR_LABELS TYPE CENTRAL PERIPHERAL"));

  		/* PERFORM CAMERA SETUP, CALIBRATION */
  eyelink->doTrackerSetup();

  /* loop through trials */
  for(trial=1;trial<=NTRIALS;trial++)
    {
	  short isConnected;
	  short breakPressed;
	  eyelink->isConnected(&isConnected);
	  eyelink->breakPressed(&breakPressed);
      if(!isConnected || breakPressed)    /* drop out if link closed */
	{
	  return EL_ABORT_EXPT;
	}
				/* RUN THE TRIAL */
      i = do_gcwindow_trial(trial);
      eyelinkUtil->endRealTimeMode();

      switch(i)         	/* REPORT ANY ERRORS */
	{
	  case EL_ABORT_EXPT:        /* handle experiment abort or disconnect */
	    eyelink->sendMessage(A2W("EXPERIMENT ABORTED"));
	    return EL_ABORT_EXPT;
	  case EL_REPEAT_TRIAL:	  /* trial restart requested */
	    eyelink->sendMessage(A2W("TRIAL REPEATED"));
	    trial--;
	    break;
	  case EL_SKIP_TRIAL:	  /* skip trial */
	    eyelink->sendMessage(A2W("TRIAL ABORTED"));
	    break;
	  case EL_TRIAL_OK:          /* successful trial */
	    eyelink->sendMessage(A2W("TRIAL OK"));
	    break;
	  default:                /* other error code */
	    eyelink->sendMessage(A2W("TRIAL ERROR"));
	    break;
	}
    }  /* END OF TRIAL LOOP */
  return 0;
}

