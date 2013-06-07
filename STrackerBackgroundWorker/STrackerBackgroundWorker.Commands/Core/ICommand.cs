// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommand.cs" company="STracker">
//  Copyright (c) STracker Developers. All rights reserved.
// </copyright>
// <summary>
//  Interface that defines the contract of the Command Pattern.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace STrackerUpdater.Commands.Core
{
    /// <summary>
    /// The Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        string CommandName { get; set; }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        void Execute(string arg);
    }
}