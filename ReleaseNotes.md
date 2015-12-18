### New in 0.9.1 (Release 2015/12/16)
* closes #12 explicit ISession registration

### New in 0.9.0 (Release 2015/11/20)
* closes #11 update to latest nhibernate and nancy libraries

### New in 0.8.3 (Release 2015/05/06)
* closes #9 by implementing GetOrThrow
* closes #8 adding auto NoMatchFoundException handling for Nancy
* closes #7 renaming nancy bootstrap methods

### New in 0.8.2 (Release 2015/02/11)
* fixed issue #5 by properly implementing NHiberate fetch provider for ThenFetch and ThenFetchMany

### New in 0.8.1 (Release 2015/01/14)
* fixed nuspec dependencies for Querify.Nh project

### New in 0.8.0 (Release 2015/01/14)
* separated NHibernate repository into a separate project
* added auto configuration for string ids to use manual id assignment by default

### New in 0.7.3.0 (Release 2015/01/13)
* added Fetch and FetchMany extensions for eager fetching

### New in 0.7.2.0 (Release 2014/12/05)
* added ExecuteForAll method to Advanced query extensions

### New in 0.7.1.0 (Release 2014/12/04)
* fix for issue #3 - crash when attempting to get entity from InMemoryRepository when no items of that type have been added to the Repository yet

### New in 0.7.0.0 (Release 2014/12/04)
* implemented auto-id generation for in memory repository to improve testability

### New in 0.6.1.0 (Release 2014/11/06)
* separating NHibernate and Nancy into separate project to keep core clean

### New in 0.6.0.1 (Release 2014/11/05)
* fixing nuget deployment issues

### New in 0.6.0.0 (Release 2014/11/05)
* working toward package stability
