using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfEscapeGame.Classes;
using WpfEscapeGame.Enums;
namespace WpfEscapeGame
{
    public partial class MainWindow : Window
    {
        Room currentRoom;
        List<Item> myItems = new List<Item>();
        public MainWindow()
        {
            InitializeComponent();

            // Items
            Item key1 = new Item()
            {
                Name = "small silver key",
                Description = "A small silver key, makes me think of one I had at highschool.",
                IsPortable = true
            };

            Item key2 = new Item()
            {
                Name = "large key",
                Description = "A large key. Could this be my way out?",
                IsPortable = true
            };

            Item locker = new Item()
            {
                Name = "locker",
                Description = "A locker. I wonder what's inside.",
                IsLocked = true,
                IsPortable = false
            };
            locker.Key = key1;
            locker.HiddenItem = key2;

            Item bed = new Item()
            {
                Name = "bed",
                Description = "Just a bed. I am not tired now.",
                IsPortable = false
            };
            bed.HiddenItem = key1;

            // Bedroom
            Room bedroom = new Room()
            {
                Name = "bedroom",
                Description = "I seem to be in a medium sized bedroom. There is a locker to the left, a nice rug on the floor, and a bed to the right.",
                Image = "Images/ss-bedroom.png"
            };
            bedroom.Items.Add(new Item() { Name = "poster", Description = "A poster on the wall with a beach scene.", IsPortable = true });
            bedroom.Items.Add(new Item() { Name = "floor mat", Description = "A bit ragged floor mat.", IsPortable = true });
            bedroom.Items.Add(new Item() { Name = "chair", Description = "A plain wooden chair.", IsPortable = false });
            bedroom.Items.Add(bed);
            bedroom.Items.Add(locker);

            // Living room
            Room living = new Room()
            {
                Name = "living room",
                Description = "A cosy living room. A clock ticks on the wall. There's a potted plant in the corner.",
                Image = "Images/ss-living.png"
            };
            living.Items.Add(new Item() { Name = "armchair", Description = "A comfy brown armchair.", IsPortable = false });
            living.Items.Add(new Item() { Name = "plant", Description = "A lush green plant.", IsPortable = false });
            living.Items.Add(new Item() { Name = "wall clock", Description = "Shows the time.", IsPortable = false });
            living.Items.Add(new Item() { Name = "bookcase", Description = "Rows of dusty books.", IsPortable = false });
            living.Items.Add(new Item() { Name = "floor mat", Description = "A striped mat.", IsPortable = true });

            // Computer room
            Room computer = new Room()
            {
                Name = "computer room",
                Description = "A dimly lit room with an old Commodore computer and a big flag on the wall.",
                Image = "Images/ss-computer.png"
            };
            computer.Items.Add(new Item() { Name = "old computer", Description = "A Commodore 64. Retro!", IsPortable = false });
            computer.Items.Add(new Item() { Name = "portrait", Description = "A framed portrait.", IsPortable = false });
            computer.Items.Add(new Item() { Name = "flag", Description = "A Commodore flag.", IsPortable = false });
            computer.Items.Add(new Item() { Name = "bucket", Description = "An empty bucket.", IsPortable = true });
            computer.Items.Add(new Item() { Name = "chair", Description = "A simple wooden chair.", IsPortable = false });

            // Doors
            Door bedroomToLiving = new Door()
            {
                Name = "green door",
                Description = "A sturdy green door leading to the living room.",
                IsLocked = true,
                Key = key2,
                ToRoom = living
            };
            bedroom.Doors.Add(bedroomToLiving);

            Door livingToBedroom = new Door()
            {
                Name = "door to bedroom",
                Description = "The door back to the bedroom.",
                IsLocked = false,
                ToRoom = bedroom
            };
            living.Doors.Add(livingToBedroom);

            Door livingToComputer = new Door()
            {
                Name = "door to computer room",
                Description = "A door leading to the computer room.",
                IsLocked = false,
                ToRoom = computer
            };
            living.Doors.Add(livingToComputer);

            Door livingToOutside = new Door()
            {
                Name = "exit door",
                Description = "The main exit. It has an electronic keypad.",
                IsLocked = true,
                ToRoom = null
            };
            living.Doors.Add(livingToOutside);

            Door computerToLiving = new Door()
            {
                Name = "door to living room",
                Description = "A door leading back to the living room.",
                IsLocked = false,
                ToRoom = living
            };
            computer.Doors.Add(computerToLiving);

            // Start game
            currentRoom = bedroom;
            txtMessage.Text = "I am awake, but cannot remember who I am!? Must have been a hell of a party last night...";
            txtRoomDesc.Text = currentRoom.Description;
            UpdateUI();
        }

        private void UpdateUI()
        {
            lstRoomItems.Items.Clear();
            foreach (Item itm in currentRoom.Items)
                lstRoomItems.Items.Add(itm);

            lstDoors.Items.Clear();
            foreach (Door door in currentRoom.Doors)
                lstDoors.Items.Add(door);

            if (!string.IsNullOrEmpty(currentRoom.Image))
            {
                try
                {
                    imgRoom.Source = new BitmapImage(new System.Uri(currentRoom.Image, System.UriKind.Relative));
                }
                catch { imgRoom.Source = null; }
            }
            else
            {
                imgRoom.Source = null;
            }

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool roomSelected = lstRoomItems.SelectedValue != null;
            bool mySelected = lstMyItems.SelectedValue != null;
            bool doorSelected = lstDoors.SelectedValue != null;

            btnCheck.IsEnabled = roomSelected;
            btnPickUp.IsEnabled = roomSelected;
            btnUseOn.IsEnabled = roomSelected && mySelected;
            btnDrop.IsEnabled = mySelected;
            btnOpenWith.IsEnabled = doorSelected && mySelected;
            btnEnter.IsEnabled = doorSelected;
        }

        private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void LstDoors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            Item roomItem = (Item)lstRoomItems.SelectedItem;

            if (roomItem.IsLocked)
            {
                txtMessage.Text = $"{roomItem.Description}. It is firmly locked.";
                return;
            }

            Item foundItem = roomItem.HiddenItem;
            if (foundItem != null)
            {
                txtMessage.Text = $"Oh, look, I found a {foundItem.Name}.";
                lstMyItems.Items.Add(foundItem);
                myItems.Add(foundItem);
                roomItem.HiddenItem = null;
                return;
            }

            txtMessage.Text = roomItem.Description;
        }

        private void BtnUseOn_Click(object sender, RoutedEventArgs e)
        {
            Item myItem = (Item)lstMyItems.SelectedItem;
            Item roomItem = (Item)lstRoomItems.SelectedItem;

            if (roomItem.Key != myItem)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(MessageType.ItemDoesNotFit);
                return;
            }

            roomItem.IsLocked = false;
            roomItem.Key = null;
            myItems.Remove(myItem);
            lstMyItems.Items.Remove(myItem);
            txtMessage.Text = $"I just unlocked the {roomItem.Name}!";
        }

        private void BtnPickUp_Click(object sender, RoutedEventArgs e)
        {
            Item selItem = (Item)lstRoomItems.SelectedItem;

            if (!selItem.IsPortable)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(MessageType.ItemNotPortable);
                return;
            }

            txtMessage.Text = $"I just picked up the {selItem.Name}.";
            myItems.Add(selItem);
            lstMyItems.Items.Add(selItem);
            lstRoomItems.Items.Remove(selItem);
            currentRoom.Items.Remove(selItem);
        }

        private void BtnDrop_Click(object sender, RoutedEventArgs e)
        {
            Item selItem = (Item)lstMyItems.SelectedItem;
            if (selItem == null) return;

            txtMessage.Text = $"I dropped the {selItem.Name}.";
            myItems.Remove(selItem);
            lstMyItems.Items.Remove(selItem);
            currentRoom.Items.Add(selItem);
            lstRoomItems.Items.Add(selItem);
        }

        private void BtnOpenWith_Click(object sender, RoutedEventArgs e)
        {
            Door door = (Door)lstDoors.SelectedItem;
            Item myItem = (Item)lstMyItems.SelectedItem;

            if (door.Key != myItem)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(MessageType.DoorWrongKey);
                return;
            }

            door.IsLocked = false;
            door.Key = null;
            myItems.Remove(myItem);
            lstMyItems.Items.Remove(myItem);
            txtMessage.Text = $"I just unlocked the {door.Name}!";
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            Door door = (Door)lstDoors.SelectedItem;

            if (door.IsLocked)
            {
                txtMessage.Text = $"The {door.Name} is locked. I need a key.";
                return;
            }

            if (door.ToRoom == null)
            {
                txtMessage.Text = "I step outside... I am free! Congratulations, you escaped!";
                return;
            }

            currentRoom = door.ToRoom;
            txtRoomDesc.Text = currentRoom.Description;
            txtMessage.Text = $"I walk through the {door.Name} into the {currentRoom.Name}.";
            UpdateUI();

            // Restore inventory
            lstMyItems.Items.Clear();
            foreach (Item itm in myItems)
                lstMyItems.Items.Add(itm);
        }
    }
}