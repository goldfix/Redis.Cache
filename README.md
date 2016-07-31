![Logo](https://raw.githubusercontent.com/goldfix/Redis.Cache/master/_etc/ico_c.png)

Redis.Cache
===========

Small library for manage cache with Redis.

Feature
-------

* Language C# Framework .NET 4.x
* Support Extending and Absolute TTL (At the same time)
* Support native data type (int, string, bool, single and double)...
* ... and automatically serialization and de-serialization object.
* Support to compression.

Nuget Package
-------------

Here: https://www.nuget.org/packages/Redis.Cache/

Configuration Parameters
------------------------

Here: https://github.com/goldfix/Redis.Cache/wiki/_pages

Last Version is: 0.9.2
----------------------

Small optimization.

*Important!*

_Changed TTL and Date / Time serialization. This version is not compatible with previous versions. Is necessary to empty database (used to save cache items) and reboot your Redis Server instance._

**What?**

It is changed format TTL and Date / Time serialization of items.

* Old format TTL serialization was: hhmmss. Sample: 002000 == 20 minutes.
* New format TTL serialization is: total seconds. Sample: 1200 == 20 minutes.
* Old format Date/Time serialization was: yyyyMMddThhmmss. Sample: 20140120T010203
* New format Date/Time serialization was: yyMMddThhmmss. Sample: 140120T010203

**Why?**

This format is shorter and more compatible between Python and C#.

**Where?**

Here: https://www.nuget.org/packages/Redis.Cache/

___