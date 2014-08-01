using System.Collections.Generic;

namespace CodeFramework.Core.Data
{
    public interface IAccount
    {
        /// <summary>
        /// A unique key for this account
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        string Username { get; set; }

        /// <summary>
        /// Gets the password or OAuth, whatever works
        /// </summary>
        /// <value>The password.</value>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>The avatar URL.</value>
        string AvatarUrl { get; set; }

        /// <summary>
        /// The domain this user belongs to
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// Gets the filters
        /// </summary>
        Dictionary<string, object> Filters { get; }

		/// <summary>
		/// Gets the pinned repositories
		/// </summary>
		/// <value>The pinnned repositories.</value>
        List<PinnedRepository> PinnnedRepositories { get; }

		/// <summary>
		/// Gets or sets the default startup view.
		/// </summary>
		/// <value>The default startup view.</value>
		string DefaultStartupView { get; set; }
    }
}