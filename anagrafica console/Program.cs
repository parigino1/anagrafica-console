using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace anagrafica_console {
    class Program {
        static List<Users> userList = new List<Users>();

        static bool run = true;
        static int selectedIndex = 0, selectedUser = 0;
        static string[] optionsMenu = new string[] { "1. Aggiungi Utente", "2. Rimuovi Utente", "3. Modifica Utente", "4. Mostra Utenti", "5. Esci" };

        static void Main(string[] args) {
            while (run) {
                Console.Clear();
                Console.CursorVisible = false;
                loadUsers(); //carico gli utenti già presenti, in modo da aggiungerli alla lista
                printRectangleTitle("ANAGRAFICA");
                for (byte i = 0; i < optionsMenu.Length; i++) {
                    setCursorPosition(10, i + 3, null);
                    if (selectedIndex == i) Console.WriteLine($"-> {optionsMenu[i]}");
                    else Console.WriteLine($"{optionsMenu[i]}");
                }
                var pressedKey = Console.ReadKey();
                switch (pressedKey.Key) {
                    case ConsoleKey.DownArrow:
                        selectedIndex++;
                        if (selectedIndex > optionsMenu.Length - 1) selectedIndex = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = optionsMenu.Length - 1;
                        break;
                    case ConsoleKey.Enter:
                        switch (selectedIndex) {
                            case 0:
                                addUser();
                                break;
                            case 1:
                                selectedUser = 0;
                                if (userList.Count > 0) deleteUser();
                                else if (userList.Count == 0) {
                                    setCursorPosition(5, 13, "Non sono presenti utenti da eliminare!");
                                    Console.ReadKey();
                                }
                                break;
                            case 2:
                                selectedUser = 0;
                                if (userList.Count > 0) modifyUser();
                                else if (userList.Count == 0) {
                                    setCursorPosition(5, 13, "Non sono presenti utenti da modificare!");
                                    Console.ReadKey();
                                }
                                break;
                            case 3:
                                setCursorPosition(5, 13, "Premi qualsiasi tasto per tornare indietro");
                                printUsers(15, true, userList);
                                Console.ReadKey();
                                break;
                            case 4:
                                run = false;
                                break;
                        }
                        break;
                }
            }
        }

        static void printStartMenu() {
            for (byte i = 0; i < optionsMenu.Length; i++) {
                setCursorPosition(10, i + 3, null);
                if (selectedIndex == i) Console.WriteLine($"-> {optionsMenu[i]}");
                else Console.WriteLine($"{optionsMenu[i]}");
            }
        }
        static void printUsers(int top, bool resetColor, List<Users> list) {
            for (byte i = 0; i < list.Count; i++) {
                if (selectedUser == i) Console.ForegroundColor = ConsoleColor.Red;
                if (resetColor) Console.ResetColor();
                setCursorPosition(5, i + top, null);
                Console.WriteLine($"[{i + 1}] ID: {list[i].id}\t\tNome: {list[i].name}\t\tCognome: {list[i].surname}");
                Console.ResetColor();
            }
        }

        //handle users functions
        static string getID() {
            return Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        }
        static void addUser() {
            string name, surname;
            Console.CursorVisible = true;
            do {
                setCursorPosition(5, 13, null);
                Console.Write("Inserisci il nome dell'utente: "); name = Console.ReadLine();
            } while (string.IsNullOrEmpty(name));
            do {
                setCursorPosition(5, 14, null);
                Console.Write("Inserisci il cognome dell'utente: "); surname = Console.ReadLine();
            } while (string.IsNullOrEmpty(surname));
            userList.Add(new Users(getID(), name, surname));
            saveUsers();
        }
        static void deleteUser() {
            List<Users> sortedList = new List<Users>();
            bool runDelete = true, sorted = false;
            while (runDelete) {
                Console.Clear();
                loadUsers();
                if (!sorted) printMenu(userList); else printMenu(sortedList);
                var pressedKey = Console.ReadKey();
                switch (pressedKey.Key) {
                    case ConsoleKey.DownArrow:
                        selectedUser++;
                        if (selectedUser > userList.Count - 1) selectedUser = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        selectedUser--;
                        if (selectedUser < 0) selectedUser = userList.Count - 1;
                        break;
                    case ConsoleKey.Enter:
                        if (userList.Count > 0) { //se la lista NON è vuota, ci sono utenti da eliminare
                            userList.RemoveAt(selectedUser);
                            selectedUser--;
                            if (selectedUser < 0) selectedUser = 0;
                            saveUsers();
                        }
                        break;
                    case ConsoleKey.D1: //sort in base al nome
                        sorted = true;
                        sortedList = userList.OrderBy(o => o.name).ToList();
                        printMenu(sortedList);
                        break;
                    case ConsoleKey.D2: //sort in base al cognome
                        sorted = true;
                        sortedList = userList.OrderBy(o => o.surname).ToList();
                        printMenu(sortedList);
                        break;
                    case ConsoleKey.Escape:
                        runDelete = false;
                        break;
                }
            }

            void printMenu(List<Users> list) {
                printRectangleTitle("ANAGRAFICA");
                printStartMenu();
                printUsers(16, false, list);
                setCursorPosition(5, 13, "Seleziona un'utente e premi <Enter> per eliminaro, <Esc> per tornare indietro\n" +
                    "\t<1> per ordinare la lista in base al nome, <2> in base al cognome");
            }
        }
        static void modifyUser() {
            List<Users> sortedList = new List<Users>();
            bool runDelete = true, sorted = false;
            while (runDelete) {
                Console.Clear();
                Console.CursorVisible = false;
                loadUsers();
                if (!sorted) printMenu(userList); else printMenu(sortedList);
                var pressedKey = Console.ReadKey();
                switch (pressedKey.Key) {
                    case ConsoleKey.DownArrow:
                        selectedUser++;
                        if (selectedUser > userList.Count - 1) selectedUser = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        selectedUser--;
                        if (selectedUser < 0) selectedUser = userList.Count - 1;
                        break;
                    case ConsoleKey.Enter:
                        string newName, newSurname, currID;
                        Console.CursorVisible = true;
                        currID = userList[selectedUser].id;
                        do {
                            if (!sorted) printMenu(userList); else printMenu(sortedList);
                            setCursorPosition(5, 16, null);
                            Console.Write("Inserisci il nuovo nome: "); newName = Console.ReadLine();
                        } while (string.IsNullOrEmpty(newName));
                        do {
                            if (!sorted) printMenu(userList); else printMenu(sortedList);
                            setCursorPosition(5, 17, null);
                            Console.Write("Inserisci il nuovo cognome: "); newSurname = Console.ReadLine();
                        } while (string.IsNullOrEmpty(newSurname));
                        userList.RemoveAt(selectedUser);
                        userList.Insert(selectedUser, new Users(currID, newName, newSurname));
                        saveUsers();
                        break;
                    case ConsoleKey.D1: //sort in base al nome
                        sorted = true;
                        sortedList = userList.OrderBy(o => o.name).ToList();
                        printMenu(sortedList);
                        break;
                    case ConsoleKey.D2: //sort in base al cognome
                        sorted = true;
                        sortedList = userList.OrderBy(o => o.surname).ToList();
                        printMenu(sortedList);
                        break;
                    case ConsoleKey.Escape:
                        runDelete = false;
                        break;
                }
            }

            void printMenu(List<Users> list) {
                printRectangleTitle("ANAGRAFICA");
                printStartMenu();
                printUsers(19, false, list);
                setCursorPosition(5, 13, "Seleziona un'utente e premi <Enter> per modificarlo, <Esc> per tornare indietro\n" +
                    "\t<1> per ordinare la lista in base al nome, <2> in base al cognome");
            }
        }

        //file handle functions
        static void saveUsers() {
            using (StreamWriter sw = new StreamWriter("users.txt"))
                foreach (Users user in userList)
                    sw.WriteLine(user.ToString());
        }
        static void loadUsers() {
            if (!File.Exists("users.txt")) setCursorPosition(5, 11, $"Il file 'users.txt' non è presente, aggiungere un'utente per crearlo!");
            else {
                userList.Clear();
                string[] splitLines = File.ReadAllLines("users.txt");
                foreach (string line in splitLines) {
                    string[] userInfo = line.Split('|');
                    userList.Add(new Users(userInfo[0], userInfo[1], userInfo[2]));
                }
                setCursorPosition(5, 11, $"Utenti nella lista: {userList.Count}");
            }
        }

        //rectangle paint functions
        static void printRectangleTitle(string title) {
            setCursorPosition(5, 1, $"---------- {title} ----------");
            setCursorPosition(5, 9, $"---------- {title} ----------");
            for (byte i = 2; i < 9; i++) setCursorPosition(5, i, "|");
            for (byte i = 2; i < 9; i++) setCursorPosition(36, i, "|");
        }
        static void setCursorPosition(int left, int top, string text) {
            Console.SetCursorPosition(left, top);
            Console.Write(text);
        }
    }
}