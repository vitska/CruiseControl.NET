# CruiseControl.NET
CruiseControl.NET is an Automated Continuous Integration server, implemented using the .NET Framework.

Fork for lightweight web dasboard, based on:
* [#Nancy](https://github.com/NancyFx/Nancy) MVC backend
* javascript SPA frontend.
* [Bootstrap v3.3.7](http://getbootstrap.com)
* [TableSorter 2.0 - Client-side table sorting with ease!](http://tablesorter.com)
* [Chartist.js 0.1.15](https://gionkunz.github.io/chartist-js/)
* [Knob: Nice, downward compatible, touchable, jQuery dial](https://github.com/aterrien/jQuery-Knob)
	
The main concern is to make tiny service (no IIS needed) minimizing bandwith needed between backend and client. On another hand enabling near realtime monitoring on slow networks. Works as a concentrator of .net remoting answers from CC.NET runners in a memory dashboard.

Checkout /project/DashboardBrokerService/ project
