/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Collections.Generic;
using System.ComponentModel;
using ME3TweaksModManager.modmanager.save.game2.FileFormats;

namespace ME3TweaksModManager.modmanager.save.game3
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [OriginalName("WeaponModSaveRecord")]
    public class WeaponMod : IUnrealSerializable, INotifyPropertyChanged
    {
        #region Fields
        [OriginalName("WeaponModClassNames")]
        private string _WeaponClassName;

        [OriginalName("WeaponClassName")]
        private List<string> _WeaponModClassNames = new List<string>();
        #endregion

        public void Serialize(IUnrealStream stream)
        {
            stream.Serialize(ref this._WeaponClassName);
            stream.Serialize(ref this._WeaponModClassNames);
        }

        #region Properties
        public string WeaponClassName
        {
            get { return this._WeaponClassName; }
            set
            {
                if (value != this._WeaponClassName)
                {
                    this._WeaponClassName = value;
                    this.NotifyPropertyChanged("WeaponClassName");
                }
            }
        }

        [Editor(
            "System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
            , typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> WeaponModClassNames
        {
            get { return this._WeaponModClassNames; }
            set
            {
                if (value != this._WeaponModClassNames)
                {
                    this._WeaponModClassNames = value;
                    this.NotifyPropertyChanged("WeaponModClassNames");
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
