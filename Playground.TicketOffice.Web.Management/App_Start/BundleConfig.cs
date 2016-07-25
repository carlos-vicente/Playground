using System.Web.Optimization;
using System.Web.Optimization.React;

namespace Playground.TicketOffice.Web.Management
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/theater", new BabelTransform())
                .Include("~/Scripts/Components/MovieTheater/MovieTheater.jsx"));
        }
    }
}