using System;
using System.IO;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using Arg = Moq.It;

namespace FeatureMirror.Specs
{
    public class FileManagerContext
    {
        protected static string FromDir;
        protected static string ToDir;
        protected static IFileManager Manager;
        protected static string SolutionDir;

        Establish context = () =>
        {
            FromDir = Path.Combine(Environment.CurrentDirectory, "From");
            ToDir = Path.Combine(Environment.CurrentDirectory, "To");
            SolutionDir = Path.GetFullPath(Environment.CurrentDirectory + "\\..\\");
            
            var calculator = new Mock<IDestinationCalculator>();
            calculator.Setup(c => c.IsValidPath(Arg.IsAny<string>())).Returns(true);
            calculator.Setup(c => c.Calculate(Arg.IsAny<string>())).Returns(ToDir);
            
            Manager = new FileManager(SolutionDir, calculator.Object);
        };

        Cleanup output = () => Directory.Delete(ToDir, true);
    }

    public class When_we_copy_a_valid_file : FileManagerContext
    {
        Because we_copied_a_file = () =>
            Manager.Copy(FromDir + "\\bin\\FeatureMirror.dll");

        It should_copy_the_file_to_the_destination = () =>
            File.Exists(ToDir + "\\From\\bin\\FeatureMirror.dll").ShouldBeTrue();
    } 

    public class When_we_copy_an_invalid_file : FileManagerContext
    {
        Because we_copied_an_invalid_file = () =>
         Manager.Copy(FromDir + "\\bin\\FeatureMirror.pdb");

        It should_copy_the_file_to_the_destination = () =>
            File.Exists(ToDir + "\\From\\bin\\FeatureMirror.pdb").ShouldBeFalse();
    }

    public class When_we_copy_files_in_a_deep_dir : FileManagerContext
    {
        Because we_copied_an_invalid_file = () =>
        {
            Manager.Copy(FromDir + "\\some\\deep\\dir\\CSs.css");
            Manager.Copy(FromDir + "\\some\\deep\\dir\\Test.js");
        };

        It should_copy_the_js_file_to_the_destination = () =>
            File.Exists(ToDir + "\\From\\some\\deep\\dir\\Test.js").ShouldBeTrue();


        It should_copy_the_css_file_to_the_destination = () =>
            File.Exists(ToDir + "\\From\\some\\deep\\dir\\CSs.css").ShouldBeTrue();
    }

    public class I_can_copy_aspx_files : FileManagerContext
    {
        Because we_copied_aspnet_files = () =>
        {
            Manager.Copy(FromDir + "\\Test.ascx");
            Manager.Copy(FromDir + "\\Test.aspx");
        };

        It should_copy_the_js_file_to_the_destination = () =>
            File.Exists(ToDir + "\\From\\Test.ascx").ShouldBeTrue();


        It should_copy_the_css_file_to_the_destination = () =>
            File.Exists(ToDir + "\\From\\Test.aspx").ShouldBeTrue();
    }
}
