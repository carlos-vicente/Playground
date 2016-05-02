using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Data.Contracts
{
    /// <summary>
    /// A contract for a SQL connection to any given datasource, where SQL commands and queries can be executed
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Execute a SQL command against the datasource
        /// </summary>
        /// <param name="sql">The SQL command to execute</param>
        /// <param name="parameters">The parameters to use when executing the command</param>
        /// <returns>A Task for the command completion</returns>
        Task ExecuteCommand(string sql, object parameters);

        /// <summary>
        /// Execute a SQL command against the datasource (executing a stored procedure)
        /// </summary>
        /// <param name="storedProcedure">The stored procedure name to execute</param>
        /// <param name="parameters">The parameters to use when executing the stored procedure</param>
        /// <returns>A Task for the command completion</returns>
        Task ExecuteCommandAsStoredProcedure(string storedProcedure, object parameters);

        /// <summary>
        /// Executes a SQL query against the datasource which returns a single result
        /// </summary>
        /// <typeparam name="T">The type to map the results to</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="parameters">The parameters to use when executing the query</param>
        /// <returns>A Task for the query's result</returns>
        Task<T> ExecuteQuerySingle<T>(string sql, object parameters);

        /// <summary>
        /// Executes a SQL query against the datasource which returns a single result (executing a stored procedure)
        /// </summary>
        /// <typeparam name="T">The type to map the results to</typeparam>
        /// <param name="storedProcedure">The stored procedure name to execute</param>
        /// <param name="parameters">The parameters to use when executing the stored procedure</param>
        /// <returns>A Task for the query's result</returns>
        Task<T> ExecuteQuerySingleAsStoredProcedure<T>(string storedProcedure, object parameters);

        /// <summary>
        /// Executes a SQL query against the datasource which returns multipe results
        /// </summary>
        /// <typeparam name="T">The type to map the results to</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="parameters">The parameters to use when executing the query</param>
        /// <returns>A Task for the query's result</returns>
        Task<IEnumerable<T>> ExecuteQueryMultiple<T>(string sql, object parameters);

        /// <summary>
        /// Executes a SQL query against the datasource which returns multipe results (executing a stored procedure)
        /// </summary>
        /// <typeparam name="T">The type to map the results to</typeparam>
        /// <param name="storedProcedure">The stored procedure name to execute</param>
        /// <param name="parameters">The parameters to use when executing the stored procedure</param>
        /// <returns>A Task for the query's result</returns>
        Task<IEnumerable<T>> ExecuteQueryMultipleAsStoredProcedure<T>(string storedProcedure, object parameters);
    }
}