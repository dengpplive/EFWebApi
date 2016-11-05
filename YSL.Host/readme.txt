Install-Package Microsoft.AspNet.WebApi.OwinSelfHost
【跨域问题】
1.Install-Package Microsoft.AspNet.WebApi.Cors
Install-Package WebAPIContrib
2.启用EnableCors()
3.在需要跨域的ApiController或者action方法上应用如下特性即可
[EnableCors(origins: "授权地址", headers: "*", methods: "*")]

XCode:
http://blog.csdn.net/asxinyu_usst/article/details/50703498