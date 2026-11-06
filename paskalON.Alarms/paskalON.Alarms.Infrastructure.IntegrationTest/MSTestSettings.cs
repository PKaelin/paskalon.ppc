// Every test uses its own temporary database, hence the tests can run in parallel.
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
