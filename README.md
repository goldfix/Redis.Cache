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

Last Version is: 0.9.0
----------------------

Small optimization.

*Important!*

_Changed serialization TTL. This version is not compatible with previous versions._
_Is necessary to empty database (used to save cache items) and reboot your Redis Server instance._

What changed?

It is changed format serialization of TTL of items.

Old format Serialization TTL was: hhmmss. Sample: 002000 == 20 minutes.
New format Serialization TTL is: total seconds. Sample: 1200 == 20 minutes.

* Why?

This format is shorter and more compatible between Python and C#.
