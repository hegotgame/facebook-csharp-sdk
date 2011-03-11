// --------------------------------
// <copyright file="FacebookApp.cs" company="Facebook C# SDK">
//     Microsoft Public License (Ms-PL)
// </copyright>
// <author>Nathan Totten (ntotten.com) and Jim Zimmerman (jimzimmerman.com)</author>
// <license>Released under the terms of the Microsoft Public License (Ms-PL)</license>
// <website>http://facebooksdk.codeplex.com</website>
// ---------------------------------

namespace Facebook
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the core Facebook functionality.
    /// </summary>
    public class FacebookApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookApp"/> class.
        /// </summary>
        public FacebookApp()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookApp"/> class.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        public FacebookApp(string accessToken)
        {
            Contract.Requires(!String.IsNullOrEmpty(accessToken));

            AccessToken = accessToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookApp"/> class.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public FacebookApp(string appId, string appSecret)
        {
            Contract.Requires(!String.IsNullOrEmpty(appId));
            Contract.Requires(!String.IsNullOrEmpty(appSecret));

            AppId = appId;
            AppSecret = appSecret;
            AccessToken = String.Concat(appId, "|", appSecret);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookApp"/> class.
        /// </summary>
        /// <param name="facebookApplication">
        /// The facebook application.
        /// </param>
        public FacebookApp(IFacebookApplication facebookApplication)
        {
            if (facebookApplication != null)
            {
                if (!string.IsNullOrEmpty(facebookApplication.AppId) && !string.IsNullOrEmpty(facebookApplication.AppSecret))
                {
                    AppId = facebookApplication.AppId;
                    AppSecret = facebookApplication.AppSecret;
                    AccessToken = string.Concat(facebookApplication.AppId, "|", facebookApplication.AppSecret);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Application ID.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the Application API Secret.
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }

        #region ApiMethods

#if(!SILVERLIGHT) // Silverlight should only have async calls

        protected T Api<T>(string path, IDictionary<string, object> parameters, HttpMethod httpMethod)
        {
            return (T)Api(path, parameters, httpMethod, typeof(T));
        }

        internal protected virtual object Api(string path, IDictionary<string, object> parameters, HttpMethod httpMethod, Type resultType)
        {
            var facebookClient = GetFacebookClient();

            return facebookClient.Api(path, parameters, httpMethod, resultType);
        }

#endif
        internal protected virtual void ApiAsync(string path, IDictionary<string, object> parameters, HttpMethod httpMethod, FacebookAsyncCallback callback, object userToken)
        {
            var facebookClient = GetFacebookClient();

            if (callback != null)
            {
                switch (httpMethod)
                {
                    case HttpMethod.Get:
                        facebookClient.GetCompleted += (o, e) => callback(new FacebookAsyncResult(e.GetResultData(), e.UserState, null, false, true, e.Error as FacebookApiException));
                        break;
                    case HttpMethod.Post:
                        facebookClient.PostCompleted += (o, e) => callback(new FacebookAsyncResult(e.GetResultData(), e.UserState, null, false, true, e.Error as FacebookApiException));
                        break;
                    case HttpMethod.Delete:
                        facebookClient.DeleteCompleted += (o, e) => callback(new FacebookAsyncResult(e.GetResultData(), e.UserState, null, false, true, e.Error as FacebookApiException));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("httpMethod");
                }
            }

            facebookClient.ApiAsync(path, parameters, httpMethod, userToken);
        }

        [Obsolete("Marked for removal.")]
        internal protected virtual void ApiAsync<T>(string path, IDictionary<string, object> parameters, HttpMethod httpMethod, FacebookAsyncCallback<T> callback, object state)
        {
            ApiAsync(
                path,
                parameters,
                httpMethod,
                ar =>
                {
                    if (callback != null)
                    {
                        callback(new FacebookAsyncResult<T>(ar.Result, ar.AsyncState, ar.AsyncWaitHandle, ar.CompletedSynchronously, ar.IsCompleted, ar.Error));
                    }
                },
                state);
        }

        #endregion

        #region Api Get/Post/Delete methods

#if (!SILVERLIGHT)

        /// <summary>
        /// Makes a DELETE request to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <returns>
        /// The json result.
        /// </returns>
        /// <exception cref="Facebook.FacebookApiException" />
        public object Delete(string path)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            return Api(path, null, HttpMethod.Delete, null);
        }

        /// <summary>
        /// Makes a DELETE request to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The json result.
        /// </returns>
        /// <exception cref="Facebook.FacebookApiException" />
        public object Delete(string path, IDictionary<string, object> parameters)
        {
            return Api(path, parameters, HttpMethod.Delete, null);
        }

        /// <summary>
        /// Makes a GET requst to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <returns>
        /// The json result.
        /// </returns>
        /// <exception cref="Facebook.FacebookApiException"/>
        public object Get(string path)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            return Api(path, null, HttpMethod.Get, null);
        }

        /// <summary>
        /// Makes a GET request to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException"/>
        /// <returns>
        /// The json result.
        /// </returns>
        public object Get(string path, IDictionary<string, object> parameters)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            return Api(path, parameters, HttpMethod.Get, null);
        }

        /// <summary>
        /// Makes a GET request to the Facebook server.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException"/>
        /// <returns>
        /// The json result.
        /// </returns>
        public object Get(IDictionary<string, object> parameters)
        {
            Contract.Requires(parameters != null);

            return Api(null, parameters, HttpMethod.Get, null);
        }

        /// <summary>
        /// Makes a GET request to the Facebook server.
        /// </summary>
        /// <typeparam name="T">
        /// The result of the API call.
        /// </typeparam>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException"/>
        /// <returns>
        /// The json result.
        /// </returns>
        public T Get<T>(string path)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            return Api<T>(path, null, HttpMethod.Get);
        }

        /// <summary>
        /// Makes a GET request to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="T">
        /// The result of the API call.
        /// </typeparam>
        /// <exception cref="Facebook.FacebookApiException"/>
        /// <returns>
        /// The json result.
        /// </returns>
        public T Get<T>(string path, IDictionary<string, object> parameters)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            return Api<T>(path, parameters, HttpMethod.Get);
        }

        /// <summary>
        /// Makes a GET request to the Facebook server.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="T">
        /// The result of the API call.
        /// </typeparam>
        /// <exception cref="Facebook.FacebookApiException" />
        /// <returns>
        /// The json result.
        /// </returns>
        public T Get<T>(IDictionary<string, object> parameters)
        {
            Contract.Requires(parameters != null);

            return Api<T>(null, parameters, HttpMethod.Get);
        }

        /// <summary>
        /// Makes a POST request to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException" />
        /// <returns>
        /// The jon result.
        /// </returns>
        public object Post(string path, IDictionary<string, object> parameters)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            return Api(path, parameters, HttpMethod.Post, null);
        }

        /// <summary>
        /// Makes a POST request to the Facebook server.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException" />
        /// <returns>
        /// The json result.
        /// </returns>
        public object Post(IDictionary<string, object> parameters)
        {
            Contract.Requires(parameters != null);

            return Api(null, parameters, HttpMethod.Post, null);
        }


        /// <summary>
        /// Makes a POST request to the Facebook server.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException" />
        /// <returns>
        /// The json result.
        /// </returns>
        public object Post(object parameters)
        {
            Contract.Requires(parameters != null);

            return Post(FacebookUtils.ToDictionary(parameters));
        }

        /// <summary>
        /// Makes a POST request to the Facebook server.
        /// </summary>
        /// <param name="path">
        /// The resource path.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Facebook.FacebookApiException" />
        /// <returns>
        /// The json result.
        /// </returns>
        public object Post(string path, object parameters)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            return Post(path, FacebookUtils.ToDictionary(parameters));
        }

#endif
        #endregion

        #region Async Get/Post/Delete methods

        public void DeleteAsync(string path, FacebookAsyncCallback callback)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            ApiAsync(path, null, HttpMethod.Delete, callback, null);
        }

        public void DeleteAsync(string path, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            ApiAsync(path, null, HttpMethod.Delete, callback, state);
        }

        public void DeleteAsync(string path, IDictionary<string, object> parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync(path, parameters, HttpMethod.Delete, callback, null);
        }

        public void DeleteAsync(string path, IDictionary<string, object> parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync(path, parameters, HttpMethod.Delete, callback, state);
        }

        public void GetAsync(string path, FacebookAsyncCallback callback)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            ApiAsync(path, null, HttpMethod.Get, callback, null);
        }

        public void GetAsync(string path, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            ApiAsync(path, null, HttpMethod.Get, callback, state);
        }

        public void GetAsync(string path, IDictionary<string, object> parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync(path, parameters, HttpMethod.Get, callback, null);
        }

        public void GetAsync(string path, IDictionary<string, object> parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync(path, parameters, HttpMethod.Get, callback, state);
        }

        public void GetAsync(IDictionary<string, object> parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(parameters != null);

            ApiAsync(null, parameters, HttpMethod.Get, callback, null);
        }

        public void GetAsync(IDictionary<string, object> parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(parameters != null);

            ApiAsync(null, parameters, HttpMethod.Get, callback, state);
        }

        public void GetAsync<T>(string path, FacebookAsyncCallback<T> callback)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            ApiAsync<T>(path, null, HttpMethod.Get, callback, null);
        }

        public void GetAsync<T>(string path, FacebookAsyncCallback<T> callback, object state)
        {
            Contract.Requires(!String.IsNullOrEmpty(path));

            ApiAsync<T>(path, null, HttpMethod.Get, callback, state);
        }

        public void GetAsync<T>(string path, IDictionary<string, object> parameters, FacebookAsyncCallback<T> callback)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync<T>(path, parameters, HttpMethod.Get, callback, null);
        }

        public void GetAsync<T>(string path, IDictionary<string, object> parameters, FacebookAsyncCallback<T> callback, object state)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync<T>(path, parameters, HttpMethod.Get, callback, state);
        }

        public void GetAsync<T>(IDictionary<string, object> parameters, FacebookAsyncCallback<T> callback)
        {
            Contract.Requires(parameters != null);

            ApiAsync<T>(null, parameters, HttpMethod.Get, callback, null);
        }

        public void GetAsync<T>(IDictionary<string, object> parameters, FacebookAsyncCallback<T> callback, object state)
        {
            Contract.Requires(parameters != null);

            ApiAsync<T>(null, parameters, HttpMethod.Get, callback, state);
        }

        public void PostAsync(string path, IDictionary<string, object> parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync(path, parameters, HttpMethod.Post, callback, null);
        }

        public void PostAsync(string path, IDictionary<string, object> parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            ApiAsync(path, parameters, HttpMethod.Post, callback, state);
        }

        public void PostAsync(IDictionary<string, object> parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(parameters != null);

            ApiAsync(null, parameters, HttpMethod.Post, callback, null);
        }

        public void PostAsync(IDictionary<string, object> parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(parameters != null);

            ApiAsync(null, parameters, HttpMethod.Post, callback, state);
        }

        public void PostAsync(string path, object parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            PostAsync(path, FacebookUtils.ToDictionary(parameters), callback, state);
        }

        public void PostAsync(object parameters, FacebookAsyncCallback callback, object state)
        {
            Contract.Requires(parameters != null);

            PostAsync(FacebookUtils.ToDictionary(parameters), callback, state);
        }

        public void PostAsync(object parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(parameters != null);

            PostAsync(FacebookUtils.ToDictionary(parameters), callback, null);
        }

        public void PostAsync(string path, object parameters, FacebookAsyncCallback callback)
        {
            Contract.Requires(!(String.IsNullOrEmpty(path) && parameters == null));

            PostAsync(path, FacebookUtils.ToDictionary(parameters), callback, null);
        }

        #endregion

        /// <summary>
        /// Gets the facebook client
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="FacebookClient"/>.
        /// </returns>
        protected FacebookClient GetFacebookClient()
        {
            // make this a method so others can easily mock the internal FacebookClient.
            return string.IsNullOrEmpty(AccessToken) ? new FacebookClient() : new FacebookClient(AccessToken);
        }
    }
}