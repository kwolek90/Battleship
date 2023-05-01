namespace Battleship.Data
{
	public class Board
    {
        readonly int size = 10;
        SeaFieldState[,] Fields { get; set; }
        bool[,] HittedFields { get; set; }

        private List<Ship> AvailableShips = new List<Ship>(){
            Ship.Destroyer,
            Ship.Destroyer,
            Ship.Battleship
        };

        public List<Ship> UnusedShips;

        private int UndestroyedShips;

        public bool GameOver => UndestroyedShips <= 0;


        public Board()
        {
            UndestroyedShips = AvailableShips.Sum(x => (int)x);
            UnusedShips = AvailableShips.ToList();
            Fields = new SeaFieldState[size, size];
            HittedFields = new bool[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Fields[i, j] = SeaFieldState.Empty;
                    HittedFields[i, j] = false;
                }
            }
        }

        public bool PlaceShip(int shipSize, int column, int row, ShipDirection direction)
        {
            if (direction == ShipDirection.Vertical)
            {
                if (!CanPlaceVerticalShip(shipSize, column, row))
                    return false;
                for (int j = 0; j < shipSize; j++)
                {
                    Fields[column, row + j] = SeaFieldState.Ship;
                }
            }
            else
            {
                if (!CanPlaceHorizontalShip(shipSize, column, row))
                    return false;
                for (int j = 0; j < shipSize; j++)
                {
                    Fields[column + j, row] = SeaFieldState.Ship;
                }
            }
            return true;
        }

        internal bool CanPlaceHorizontalShip(int shipSize, int column, int row)
        {
            if (column + shipSize > size)
                return false;

            for (int k = Math.Max(0, column - 1); k < Math.Min(size, column + shipSize + 2); k++)
            {
                for (int l = Math.Max(0, row - 1); l < Math.Min(size, row + 2); l++)
                {
                    if(Fields[k, l] == SeaFieldState.Ship)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal bool CanPlaceVerticalShip(int shipSize, int column, int row)
        {
            if (row + shipSize > size)
                return false;

            for (int k = Math.Max(0,column - 1); k < Math.Min(size, column + 2); k++)
            {
                for (int l = Math.Max(0, row - 1); l < Math.Min(size, row + shipSize + 2); l++)
                {
                    if (Fields[k, l] == SeaFieldState.Ship)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void RandomlyInitialize()
        {

            foreach (var ship in AvailableShips)
            {
                int k, l, dir;
                bool canPlaceShip;
                do
                {
                    dir = System.Random.Shared.Next() % 2;

                    if (dir == 0)
                    {
                        k = System.Random.Shared.Next() % (size - (int)ship);
                        l = System.Random.Shared.Next() % size;
                        canPlaceShip = CanPlaceHorizontalShip((int)ship, k, l);
                    }
                    else
                    {
                        k = System.Random.Shared.Next() % size;
                        l = System.Random.Shared.Next() % (size - (int)ship);
                        canPlaceShip = CanPlaceVerticalShip((int)ship, k, l);
                    }
                }
                while (!canPlaceShip);

                PlaceShip((int)ship, k, l, (ShipDirection)dir);
                
            }
        }

        public void Shoot(int k, int l)
        {
            if (HittedFields[k, l])
                return;
            HittedFields[k, l] = true;
            if (Fields[k, l] == SeaFieldState.Ship)
            {
                Fields[k, l] = SeaFieldState.Hit;
                UndestroyedShips--;
            }
        }

        public SeaFieldState GetField(int k, int l)
        {
            return Fields[k, l];
        }
        public SeaFieldState GetFoWField(int k, int l)
        {
            return HittedFields[k,l] ? Fields[k, l] : SeaFieldState.Unknown;
        }

        public void ShootRandomNotShootedBox()
        {
            int k, l;

            do
            {
                k = Random.Shared.Next() % size;
                l = Random.Shared.Next() % size;
            }
            while(HittedFields[k, l]);
            Shoot(k, l);
        }


    }
}
