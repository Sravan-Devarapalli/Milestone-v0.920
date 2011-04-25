function Get_Cookie(name)
{
	var start = document.cookie.indexOf(name + "=");
	var len = start + name.length + 1;
	if ((!start) && (name != document.cookie.substring(0, name.length)))
	{
		return null;
	}
	if (start == -1) return null;
	var end = document.cookie.indexOf(";", len);
	if (end == -1) end = document.cookie.length;
	return unescape(document.cookie.substring(len, end));
};

function Delete_Cookie(name, path, domain)
{
	if (Get_Cookie(name))
	{
		var d = new Date();
		document.cookie = name + "=" + ((path) ? ";path=" + path : "") + ((domain) ? ";domain=" + domain : "") + ";expires=" + d.toGMTString() + ";" + ";";
	}
};

