﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LunaparkGame {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Notices {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Notices() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LunaparkGame.Notices", typeof(Notices).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No money to buy this..
        /// </summary>
        internal static string cannotBuyNoMoney {
            get {
                return ResourceManager.GetString("cannotBuyNoMoney", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Firstly, repair the amusement..
        /// </summary>
        internal static string cannotChangeFirstRepair {
            get {
                return ResourceManager.GetString("cannotChangeFirstRepair", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It cannot be demolished. The amusement is running..
        /// </summary>
        internal static string cannotDemolishAmusement {
            get {
                return ResourceManager.GetString("cannotDemolishAmusement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No money to repair..
        /// </summary>
        internal static string cannotRepairNoMoney {
            get {
                return ResourceManager.GetString("cannotRepairNoMoney", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you really want to close this game?.
        /// </summary>
        internal static string closeGame {
            get {
                return ResourceManager.GetString("closeGame", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your research was successful. You invented: .
        /// </summary>
        internal static string newRevealedItem {
            get {
                return ResourceManager.GetString("newRevealedItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The building has not been finnished. Finish it first..
        /// </summary>
        internal static string unfinishedBuilding {
            get {
                return ResourceManager.GetString("unfinishedBuilding", resourceCulture);
            }
        }
    }
}