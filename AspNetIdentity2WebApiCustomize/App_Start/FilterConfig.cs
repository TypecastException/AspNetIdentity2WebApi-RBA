using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity2WebApiCustomize {
  public class FilterConfig {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
      filters.Add(new HandleErrorAttribute());
    }
  }
}
