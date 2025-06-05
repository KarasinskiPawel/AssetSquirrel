namespace AssetSquirrel.WebApp.Components.ViewModels.Locations
{
    public class LocationAddDialogBoxVM
    {
        public bool Show { get; set; }

        public void ModalShow()
        {
            Show = true;
        }

        public void ModalHide()
        {
            Show = false;
        }
    }
}
