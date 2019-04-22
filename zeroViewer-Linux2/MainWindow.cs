using System;
using Gtk;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using System.Linq;
//using Gdk;


public partial class MainWindow: Gtk.Window
{	
	//index of file being displayed
	int fileindex = 0;

	//array of all files in the specified folder
	string[] FilePaths_all;

	//list of all files that can be displayed
	//(based on the conditions specified in "getNodeList")
	List<FileInfo> FilePaths_usable;

	//random number generator used in random slide show
	Random rng = new Random();

	//used in "slideshow" event
	Timer Time;

	//used in random "slideshow" event
	Timer TimeRand;

	public MainWindow(): base(Gtk.WindowType.Toplevel)
	{
		Build();

		AddEvents ();

		getNodeList("./");
		showThisItem(0);

		Time = new Timer (3000);
		Time.Enabled = false;
		Time.Elapsed += OnTimeEvent;

		TimeRand = new Timer(1000);
		TimeRand.Enabled = false;
		TimeRand.Elapsed += OnTimeRandEvent;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void AddEvents()
	{
		btnShow.Clicked += btnShow_Click;
		btnRandom.Clicked += btnRandom_Click;
		btnBackward.Clicked += btnBackward_Click;
		btnForward.Clicked += btnForward_Click;
		filechooserbutton1.SelectionChanged += OnFilechooserbutton1SelectionChanged;
		btnClose.Clicked += btnClose_Click;
	}

	/// <summary>
	/// Gets the list of files (or node list).
	/// permmitted file types are specified here
	/// </summary>
	/// <param name="folder">Folder.</param>
	private void getNodeList (string folder)
	{
		FilePaths_all = Directory.GetFiles(folder);
		FilePaths_usable = new List<FileInfo> ();

		//https://stackoverflow.com/questions/3527203/getfiles-with-multiple-extensions/3527295#3527295
		string[] extensions = {".jpg",".JPG", ".png", ".PNG", ".gif"};

		IEnumerable<FileInfo> files = new DirectoryInfo(folder).EnumerateFiles();
		FilePaths_usable =  files.Where(f => extensions.Contains(f.Extension)).ToList();
	}

	/// <summary>
	/// Shows the file specified by the index "fileindex" of the list "FilePaths_usable".
	/// </summary>
	/// <param name="number">Number.</param>
	private void showThisItem (int number)
	{ 
		if(FilePaths_usable.Count > 0)
		{
			if( FilePaths_usable[number].Extension ==  ".gif" )
				pixbox.PixbufAnimation = new Gdk.PixbufAnimation (FilePaths_usable [number].FullName);
			else
				pixbox.Pixbuf = new Gdk.Pixbuf (FilePaths_usable [number].FullName, WidthRequest, HeightRequest, true); //pixbox.WidthRequest, pixbox.HeightRequest, true);

			lblOut.Text = FilePaths_usable[number].ToString();
		}
		else
			lblOut.Text = "find some files";
	}

	//buttons
	/// <summary>
	/// Progresses through the list of files "FilePaths_usable".
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void btnForward_Click (object sender, EventArgs e)
	{
		fileindex = fileindex < FilePaths_usable.Count - 1 ? fileindex+1 : 0;

		showThisItem(fileindex);
	}

	/// <summary>
	/// Regresses through the list of files "FilePaths_usable".
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void btnBackward_Click (object sender, EventArgs e)
	{
		fileindex = 0 < fileindex ? fileindex-1 : FilePaths_usable.Count - 1;

		showThisItem(fileindex);
	}	

	/// <summary>
	/// Starts a "slideshow" of files "FilePaths_usable".
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void btnShow_Click (object sender, EventArgs e)
	{
		Debug.WriteLine ("try to start show [*,...]");
		if (!Time.Enabled) 
		{
			Time.Enabled = true;
			TimeRand.Enabled = false;
		}
		else
			Time.Enabled = false;
	}
	protected void OnTimeEvent (object sender, ElapsedEventArgs e)
	{
		btnForward_Click (this, null);
	}

	/// <summary>
	/// Starts a random "slideshow" of files "FilePaths_usable".
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void btnRandom_Click (object sender, EventArgs e)
	{
		//random show
		if (!TimeRand.Enabled) 
		{
			TimeRand.Enabled = true;
			Time.Enabled = false;
		}
		else
			TimeRand.Enabled = false;
	}
	protected void OnTimeRandEvent(object sender, ElapsedEventArgs e)
	{
		fileindex = rng.Next(FilePaths_usable.Count);

		showThisItem(fileindex);
	}	

	/// <summary>
	/// Used to change the directory of pictures being displayed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void OnFilechooserbutton1SelectionChanged (object sender, EventArgs e)
	{
		FilePaths_all.Initialize ();
		FilePaths_usable.Clear ();

		getNodeList (((FileChooserButton)sender).Filename);

		if (FilePaths_usable.Count > 0) {
			foreach (var f in FilePaths_usable)
				Debug.WriteLine (f);

			showThisItem (0);
		}
	}

	/// <summary>
	/// closes the program
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void btnClose_Click (object sender, EventArgs e)
	{
		Application.Quit();
	}	

	protected void OnFileChooserSectionChange (object sender, EventArgs e)
	{
		throw new NotImplementedException ();
	}
}
