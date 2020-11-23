This applications solves a peculiar problem.

## The Problem
I use the Windows Inverted mouse pointer theme. Unfortunately, something keeps resetting the mouse pointer.
The registry keys are untouched, it just appears that Windows forgets what theme was being used.
This happens randomly while I'm logged in and consistently happens when I wake my PC up from sleep.

## The Solution
The solution I found was that opening the mouse pointer dialog and (making no changes) clicking OK forced Windows
to reload the registry values for the mouse pointer theme. Of course this is annoying at the very least, so I
automated this effect.

Windows does 2 things when you click OK from the mouse pointer dialog box:
1. Sets the registry values for each mouse pointer in `HKEY_CURRENT_USER\Control Panel\Cursors`
2. Triggers a call to `SystemParametersInfo` in user32.dll with a UI Action of `SPI_SETCURSORS` (0x0057)

In my case, the cursor values in the registry are unchanged so step 1 can be skipped. This application triggers
the same call to user32.dll.

## Important Notes
While this is a console app and has no UI, it is set up as a WinForms application __this is by design__.
The reason to do this is to hide the console window completely (not just run minimized). When it's done
this way, there is no flicker of the console window neither on the Taskbar nor the window itself.

## Configuration
For best results, this application should be run via the task manager.
Task settings:
* Run when user is logged on (_must be set to this as it needs to interact with the user session_)
* Triggers
  * On a schedule every 15 mins
  * When workstation is unlocked
* Actions
  * Run the applicaiton (_no command line args_)

## Return status codes
* 0: Success
* -2: Unexpected command line args (_there should be none_)
* Others: Error code directly from `WinError.c`
