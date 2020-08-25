using BassClefStudio.DbLanguage.Core.Scripts.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Scripts.Threading
{
    /// <summary>
    /// Represents the pointer for a <see cref="Thread"/> that controls the movement between <see cref="ICommand"/> objects.
    /// </summary>
    public class ThreadPointer
    {
        /// <summary>
        /// The index of the current running <see cref="ICommand"/>.
        /// </summary>
        public int CurrentIndex { get; private set; }

        /// <summary>
        /// Gets the current running <see cref="ICommand"/>.
        /// </summary>
        public ICommand CurrentCommand => CurrentIndex < Commands.Length ? Commands[CurrentIndex] : null;

        /// <summary>
        /// Represents the collection of <see cref="ICommand"/> objects that will be run in a <see cref="Thread"/> and managed by the <see cref="ThreadPointer"/>.
        /// </summary>
        public ICommand[] Commands { get; }

        /// <summary>
        /// Represents certain flags that can be triggered by events happening outside of the <see cref="ThreadPointer"/> (such as <see cref="Thread"/> actions and <see cref="ICommand"/> execution), or internal triggers such as the value of <see cref="CurrentIndex"/>.
        /// </summary>
        public CommandPointerFlags Flags { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ThreadPointer"/> with a collection of <see cref="ICommand"/> objects to manage.
        /// </summary>
        /// <param name="commands">The collection of <see cref="ICommand"/> objects that will be run by the <see cref="Thread"/>.</param>
        public ThreadPointer(params ICommand[] commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether any of the <see cref="CommandPointerFlags"/> have been set that indicate that the <see cref="Thread"/> should stop execution.
        /// </summary>
        public bool IsStopped
        {
            get
            {
                return Flags.HasFlag(CommandPointerFlags.Complete)
                    || Flags.HasFlag(CommandPointerFlags.Returned);
            }
        }

        /// <summary>
        /// Adds the given flags to the <see cref="ThreadPointer"/>'s <see cref="Flags"/> property.
        /// </summary>
        /// <param name="flags">The given flags to add.</param>
        public void AddFlags(CommandPointerFlags flags)
        {
            Flags |= flags;
        }

        /// <summary>
        /// Removes the given flags from the <see cref="ThreadPointer"/>'s <see cref="Flags"/> property.
        /// </summary>
        /// <param name="flags">The given flags to add.</param>
        public void RemoveFlags(CommandPointerFlags flags)
        {
            ////Note: Removing a flag is done by ANDing the complement:
            ////Flags &= ~(CommandPointerFlags.CompletedCommands);
            ////You can also OR commands together before taking the complement to remove multiple flags from the value.
            Flags &= ~flags;
        }

        /// <summary>
        /// Moves the pointer's <see cref="CurrentCommand"/> to the next <see cref="ICommand"/> and sets/checks relevant flags.
        /// </summary>
        public void Next()
        {
            CurrentIndex++;
            if(CurrentIndex == Commands.Length)
            {
                AddFlags(CommandPointerFlags.Complete);
            }
        }
    }

    /// <summary>
    /// Represents a list of flags for a <see cref="ThreadPointer"/> that specify the different ways in which execution of a <see cref="Thread"/> could end.
    /// </summary>
    [Flags]
    public enum CommandPointerFlags
    {
        /// <summary>
        /// Indicates that the entire collection of <see cref="ThreadPointer.Commands"/> completed successfully.
        /// </summary>
        Complete = 1,

        /// <summary>
        /// Indicates that a value was returned from an <see cref="ICommand"/> in the <see cref="ThreadPointer.Commands"/> collection.
        /// </summary>
        Returned = 2
    }
}
