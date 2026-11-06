// Every test uses its own temporary database and its own exchange, hence the tests can run in parallel.
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
