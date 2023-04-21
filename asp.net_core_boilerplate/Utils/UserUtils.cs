using System;
namespace asp.net_core_boilerplte.Utils
{
	public static class UserUtils
	{
		public static string GetDisplayName(System.Security.Claims.ClaimsPrincipal user)
		{
			try
			{
				return user?.Claims?.Where(claim =>
					claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname" ||
					claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname" ||
					claim.Type == "username"
				)?.First()?.Value ?? "";
			}
			catch (Exception)
			{
				return "";
			}
		}
	}
}

