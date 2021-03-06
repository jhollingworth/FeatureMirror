using System;
using System.IO;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace FeatureMirror
{
	public sealed class Connect : IDTExtensibility2, IDTCommandTarget
	{
        internal static DTE2 Application;
        internal AddIn AddIn;
        private SolutionMirror _solutionMirror;

		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
            Logger.Log("Wibble");
		    try
		    {
                Application = (DTE2)application;
		    }
		    catch (Exception ex)
		    {
                Logger.Log("An error occured while getting the application: {0}", ex.Message);
                Logger.Log("Application is {0}", application.GetType().FullName);
		        return;
		    }

		    try
		    {
		        AddIn = (AddIn) addInInst;
		    }
		    catch (Exception ex)
		    {
                Logger.Log("An error occured while getting the addin: {0}", ex.Message);
                Logger.Log("Addin is {0}", addInInst.GetType().FullName);
                return;
		    }

		    try
		    {
		        InitialiseFeatureCopyMenu();
		    }
            catch(Exception ex)
            {
                Logger.Log("An error occured while initialize the copy feature: {0}", ex.Message);
                return;
            }

		    try
		    {
		        Application.Events.SolutionEvents.Opened += SolutionOpened;
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while adding the opened event: {0}", ex.Message);
                return;
            }

            Logger.Log("Connected to Vs");
        }

        private const string COMMAND_NAME = "Copy";
        private CommandBarPopup _featureCopy;
	    private IFeatureCopier _featureCopier;

	    private void InitialiseFeatureCopyMenu()
	    {
	        Command copyCommand = null;

            try
            {
                copyCommand = GetCommand();
            }
            catch (Exception ex)
            {
                Logger.Log("Failed getting the command: {0}", ex.Message);
                return;
            }

            foreach (var windowType in new[] { "Code Window" })
            {
                try
                {
                    AddCommandToWindow(copyCommand, windowType);
                }
                catch (Exception ex)
                {
                    Logger.Log("An error occured while initializing the menu: {0}", ex.Message);
                }
            }
        }

	    private void AddCommandToWindow(Command copyCommand, string windowType)
	    {
	        var commandBars = (CommandBars)Application.CommandBars;
            var codeWindowCommandBar = commandBars[windowType];

	        _featureCopy = (CommandBarPopup) codeWindowCommandBar.Controls.Add(MsoControlType.msoControlPopup,
	                                                                           Type.Missing, Type.Missing, Type.Missing, Type.Missing);
	        _featureCopy.Caption = "Feature";

	        var copyButton = (CommandBarControl)copyCommand.AddControl(_featureCopy.CommandBar,
	                                                                   _featureCopy.Controls.Count + 1);

	        copyButton.Caption = "Copy";
	        copyButton.TooltipText = "Copy's all feature resources to main application";
                
	        Logger.Log("Menu has been initailized");
	    }

	    private Command GetCommand()
	    {
	        Command command = null;
             
            try
            {
                command = Application.Commands.Item(AddIn.ProgID + "." + COMMAND_NAME, -1);
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while getting the command from the application: {0}", ex.Message);
            }

            if(command != null)
            {
                command.Delete();
            }

            try
            {
                object[] contextUIGuids = new object[] { };

                return Application.Commands.AddNamedCommand(AddIn, COMMAND_NAME, "Copy", "Copy the whole feature", true, 1,
                   ref contextUIGuids, (int)vsCommandStatus.vsCommandStatusSupported);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed when adding named command: {0}", ex.Message);
                throw;
            }
        }

	    private void SolutionOpened()
        {
            var solution = Application.Solution;
            var solutionName = solution.FullName;
            var solutionDir = GetSolutionDir();

            if (false == IsFeature())
            {
                Logger.Log("This solution is not a feature so not mirroring");
                return;
            }

            Logger.Log("The solution {0} has been opened. It is found here {1}", 
                solutionName, 
                solutionDir
            );

	        try
	        {
                _featureCopier = GetFeatureCopier();
                _featureCopier.Copy();
	        }
	        catch (Exception ex)
	        {
                Logger.Log("An error occured while copying the feature: {0}", ex.Message);
	        }

            try
            {
                _solutionMirror = new SolutionMirror(
                    solutionName,
                    solutionDir,
                    new FileWatcher(),
                    GetFileManager()
                );
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while setting up the solution mirror: {0}", ex.Message);
            }
        }

		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
            Logger.Log("The plugin is being disconnected");

            if(_solutionMirror != null)
            {
                _solutionMirror.Dispose();
            }
		}

        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            Logger.Log("Copying the feature");
            GetFeatureCopier().Copy();
        }

        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if(IsFeature())
            {
                status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            }
            else
            {
                status = vsCommandStatus.vsCommandStatusSupported;
            } 
        }

        private bool IsFeature()
        {
            return GetSolutionDir().GetAllInstancesOf("feature.yml").Count > 0;
        }

        private string GetSolutionDir()
        {
            return Path.GetDirectoryName(Application.Solution.FileName);
        }

        public IFileManager GetFileManager()
        {
            return new FileManager(GetSolutionDir(), new DestinationCalculator());
        }

        public IFeatureCopier GetFeatureCopier()
        {
            return new FeatureCopier(
                GetFileManager(),
                GetSolutionDir()
            );
        }

        #region redundant
        public void OnAddInsUpdate(ref Array custom)
        {
        }
        public void OnStartupComplete(ref Array custom)
        {
        }
        public void OnBeginShutdown(ref Array custom)
        {
        }
        #endregion
	}
}