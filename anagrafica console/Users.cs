namespace anagrafica_console {
    class Users {
        private string _id, _name, _surname;
        public string id {
            get { return _id; }
            set { _id = value; }
        }
        public string name {
            get { return _name; }
            set { _name = value; }
        }
        public string surname {
            get { return _surname; }
            set { _surname = value; }
        }

        //costruttore
        public Users(string id, string name, string surname) {
            this.id = id; this.name = name; this.surname = surname;
        }

        //override .toString()
        public override string ToString() {
            return id + '|' + name + '|' + surname;
        }
    }
}