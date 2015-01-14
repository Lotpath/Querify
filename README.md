Querify
===

Querify provides helpful abstractions and extensions over NHibernate such as:

* A powerful Repository abstraction that simplifies data access and testing scenarios
* Extension methods on IQueryable that guide you toward the pit of success (paging instead of "fetch all" scenarios, automatic lazy loading and multi-fetch)
* A flexible specification based querying approach for building up complicated Linq queries that are easy for client code to invoke without bleeding nasty Linq expressions further up your stack
* Eager fetching abstraction that keeps NHibernate out of your core domain while allowing you to leverage Eager fetching (preventing n+1 problems)

It is recommended that you not work directly with an IRepository outside of your core domain, but instead implement 
service, manager and engine classes in your core domain which take a dependency on IRepository. Client code of your domain
should interact with your services, managers and engines rather than directly with the IRepository abstraction.

Querify fully embraces the concept of an Onion Architecture where your core Domain is the center of the Onion, and the 
IRepository is merely a service (or a Port/Adapter in Hexagonal architecture parlance), making data access a service rather than the "base" 
or "foundation" of your application.

Usage
---

Querify is available on NuGet: 

* [Querify](https://www.nuget.org/packages/Querify/)
* [Querify.Nh](https://www.nuget.org/packages/Querify.Nh/)
* [Querify.Nancy.Nh](https://www.nuget.org/packages/Querify.Nancy.Nh/)

Querify is the core project, intended to be referenced by your core domain project.

Querify.Nh provides a repository implementation for [NHibernate](http://nhforge.org/)

Querify.Nancy.Nh provides hooks to configure a [Nancy](http://nancyfx.org/) application to 
work with [NHibernate](http://nhforge.org/) and Querify

Possible Future Enhancements
---

* An example domain project, highlighting how you can leverage the onion architecture approach and test your domain with the InMemoryRepository
* Improved unit and integration test coverage
* _maybe_ an EntityFramework implementation of IRepository (I did said _maybe_)
