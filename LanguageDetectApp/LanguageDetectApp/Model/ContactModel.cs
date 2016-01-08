using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;


namespace LanguageDetectApp.Model
{
    public class ContactModel :INotifyPropertyChanged
    {
        //private StoredContact _storedContact;

        #region Private Attribute
        private string _givenName;

        private string _telephone;

        private string _familyName;

        private string _id;

        private string _remoteId;

        private string _address;

        private DateTime _birthday;

        private string _nickname;

        private StorageFile _thumbnail;

        private string _mobilephone;

        private string _alternate_mobilephone;

        private bool _pageCollapsed;

        private string _street;

        private string _country;

        private string _region;

        private string _email;
        #endregion

        #region Property
        public string Id
        {
            get { return _id; }
            set
            {
                SetProperty<string>(ref _id, value, "Id");
            }
        }

        public string RemoteId
        {
            get { return _remoteId; }
            set
            {
                SetProperty<string>(ref _remoteId, value, "RemoteId");
            }
        }

        public string GivenName
        {
            get { return _givenName; }
            set
            {
                SetProperty<string>(ref _givenName, value, "GivenName");
            }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set {
                SetProperty<string>(ref _familyName, value, "FamilyName");
            }
        }

        public string Nickname
        {
            get { return _nickname; }
            set
            {
                SetProperty<string>(ref _nickname, value, "Nickname");
            }
        }

        public string Mobilephone
        {
            get { return _mobilephone; }
            set
            {
                SetProperty<string>(ref _mobilephone, value, "Mobilephone");
            }
        }

        public string AlternateMobilePhone
        {
            get { return _alternate_mobilephone; }
            set
            {
                SetProperty<string>(ref _alternate_mobilephone, value, "AlternateMobilePhone");
            }
        }

        public string Telephone
        {
            get { return _telephone; }
            set
            {
                SetProperty<string>(ref _telephone, value, "Telephone");
            }
        }

        public string Address
        {
            get { return _address; }
            set {
                SetProperty<string>(ref _address, value, "Address");
            }
        }

        public string Street
        {
            get { return _street; }
            set { SetProperty<string>(ref _street, value, "Street"); }
        }

        public string Region
        {
            get { return _region; }
            set {
                SetProperty<string>(ref _region, value, "Region");
            }
        }

        public string Country
        {
            get { return _country; }
            set {
                SetProperty<string>(ref _country, value, "Country");
            }
        }

        public DateTime Birthday
        {
            get { return _birthday; }
            set {
                SetProperty<DateTime>(ref _birthday, value, "Birthday");
            }
        }

        public StorageFile Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                SetProperty<StorageFile>(ref _thumbnail, value, "Thumbnail");
            }
        }

        public bool PageCollapsed
        {
            get { return _pageCollapsed; }
            set
            {
                SetProperty<bool>(ref _pageCollapsed, value, "PageCollapsed");
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                SetProperty<string>(ref _email, value, "Email");
            }
        }
#endregion

        #region Constructor
        //public ContactModel(StoredContact contact)
        //{
        //    _storedContact = contact;
        //}
        public ContactModel()
        {
            this.Address = string.Empty;
            this.AlternateMobilePhone = string.Empty;
            this.Birthday = DateTime.Now;
            this.FamilyName = string.Empty;
            this.GivenName = string.Empty;
            this.Id = string.Empty;
            this.Mobilephone = string.Empty;
            this.Nickname = string.Empty;
            this.PageCollapsed = false;
            this.RemoteId = string.Empty;
            this.Telephone = string.Empty;
            this.Thumbnail = null;

        }        
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private bool SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (Object.Equals(field, value) == true)
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
      
        #endregion

        public string GenerateLocalId()
        {
            string localId = string.Empty;

            #region Random Hex Id 
            do
            {
                Random random = new Random();
                int num = random.Next(0, int.MaxValue);
                localId += num.ToString("X8");
              
            } while (isExistsLocalId(localId));

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Add(localId, true);
            #endregion // Random Hex Id

            return localId;
        }

        public async Task<StoredContact> ToStoredContact()
        {
            StoredContact contact = new StoredContact(App.ContactStore);

            contact.RemoteId = this.GenerateLocalId();
            contact.RemoteId = await App.RemoteIdHelper.GetTaggedRemoteId(App.ContactStore, contact.RemoteId);

            if (String.IsNullOrEmpty(this.GivenName) == false)
                contact.GivenName = this.GivenName;

            if (String.IsNullOrEmpty(this.FamilyName) == false)
                contact.FamilyName = this.FamilyName;


            #region Set Picture ; Test = not yet
            if (this.Thumbnail != null)
            {
                using (IInputStream inStream = await this.Thumbnail.OpenSequentialReadAsync())
	            {
                    await contact.SetDisplayPictureAsync(inStream);	 
	            }
            }
            #endregion


            var props = await contact.GetPropertiesAsync();
            if (String.IsNullOrEmpty(this.Nickname) == false)
                props.Add(KnownContactProperties.Nickname, this.Nickname);
            if (String.IsNullOrEmpty(this.Mobilephone) == false)
                props.Add(KnownContactProperties.MobileTelephone, this.Mobilephone);
            if (String.IsNullOrEmpty(this.AlternateMobilePhone) == false)
                props.Add(KnownContactProperties.AlternateMobileTelephone, this.AlternateMobilePhone);
            if (String.IsNullOrEmpty(this.Telephone) == false)
                props.Add(KnownContactProperties.Telephone, this.Telephone);
            if (string.IsNullOrEmpty(this.Email) == false)
                props.Add(KnownContactProperties.Email, this.Email);
            if (String.IsNullOrEmpty(this.Address) == false)
                props.Add(KnownContactProperties.Address, new ContactAddress()
                {
                    Locality = this.Address,
                    StreetAddress =  (this.Street == null) ? "":this.Street,
                    Region = (this.Region == null) ? "" : this.Region,
                    Country = (this.Country == null) ? "" : this.Country
                }
                );

            if (this.Birthday != null)
                props.Add(
                    KnownContactProperties.Birthdate, 
                    new DateTimeOffset(
                        new DateTime(this.Birthday.Year, this.Birthday.Month, this.Birthday.Day),
                        new TimeSpan(1, 0, 0)));

            return contact;
        }

        // Kiểm tra xem app có id nào giống @localid không.
        // return @true nếu có,, return @false nếu không.
        private bool isExistsLocalId(string localid)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            return localSettings.Values.ContainsKey(localid);
        }


    }
}
