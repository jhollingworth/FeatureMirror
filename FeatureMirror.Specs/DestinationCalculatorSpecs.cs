using Machine.Specifications;

namespace FeatureMirror.Specs
{
    public class DestinationCalculatorContext
    {
        protected static string Destination;
        protected static string Path;
        protected static DestinationCalculator Subject;

        Establish context = () =>
            Subject = new DestinationCalculator();

        protected static void calculated_the_destination()
        {
            Destination = Subject.Calculate(Path);
        }
    }

    public class When_the_project_is_admin :  DestinationCalculatorContext
    {
        Establish context = () =>
            Path = "d:\\dev\\trunk\\web\\toptable.Lists\\app\\toptable.Lists.Admin\\Scripts\\foo.js";

        Because we = calculated_the_destination;

        It should_go_to_the_content_app = () =>
            Destination.ShouldEqual("d:\\dev\\trunk\\web\\toptable.Content\\app\\toptable.Content.Web");
    }

    public class When_the_project_is_web : DestinationCalculatorContext
    {
        Establish context = () =>
            Path = "d:\\dev\\trunk\\web\\toptable.Lists\\app\\toptable.Lists.Web\\Scripts\\foo.js";

        Because we = calculated_the_destination;

        It should_go_to_the_site = () =>
            Destination.ShouldEqual("d:\\dev\\trunk\\Site\\Web");
    }
}