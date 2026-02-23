# college-admin-app

#Sample Documentation#
Added these notes to the top of the .cs
A bit sloppy, could use some restructuring, and a few more methods to copy reused code.

# Recent Bug fixes / Features since last update
- Added labels to what menu user is in when typing in (some where not obvious)
- Fixed double EntertoContinue when search by id retunred null
- Added Personalized exit typing when adding marks, add no marks, exit says abort. add 1+ marks says done with marking
- Added exit early (typing exit) for removing marks, updating marks.
- Message on start up warrning french and spanish users to use period as decimal point and not a comma.

# New Features
- Now uses dictionarys to store all consoleWrite strings, and is targeted by an Enum Key
- Providing language Support For
    - English
    - Spanish
    - French
- New menu for selecting language
- Changed mark type from double to float
- Cleaned up confirmation flows
- fixed using a nested loop where I used choice twice, badinput message was being skipped.
- fixed null ordering in condition such as if ( student.Marks.Length == 0 || student.Marks == null )
- type with mark: in MessageEnum.markFormat, "[Index: {0}, Mark: {1}]" } was missing colon

# Key Features
- The program loads from a file at the start and introduces a small animated timeout.
- Update student information.
    - Select user by name or ID.
    - Provide a helper option to list all users.
    - Update Marks Menu (if no marks, defaults to adding marks)
        - View Student Details, including Name and Marks
        - Update student's name.
        - Update students' marks.
            - View student marks.
            - Update specific marks based on index (shows list).
            - Add new marks (you can keep adding until you type exit).
            - Remove marks based on index (shows list).
    - Options to exit these menus
- Select user by name.
    - Finding multiple names returns a list of all matching students and their IDs, which then allows the user to type in their ID based on the helpful list provided.
    - Type the name of a student to search if a name contains that.
    - accepts regex commands to allow the user to search names better
    - Timeout on regex set to prevent recursive searching (a+)+$; odd grouping, though not likely.
    - Prevent long regex by limiting length to 50.
    - The regex case insensitive gives the user hints on commands, such as basics and how to override insensitivity (?-i:CaseSensative).
    - If searching for "Eric," it can match Eric, Eric Beaudoin, Eric, and other variations. All are returned in a neat list.
    - The user can perform such searches as ^(?-i:eric)$ to specifically return only "eric" in lowercase. If there are two "Eric," it will return a list.
    - The user can exit by typing "exit."
- Improved color Coding
- Additional options to save the file to disk and encrypt it with a password
    - Encryption is lacking because .NET can be reverse engineered and passwords captured. Good enough for now, though.
    - Loads file on program start if it exists
    - Two new menu options to load and save files
    - saves the id_count variable so that when a unique ID is reached, the loaded list retains that even if there were students removed. especially at the end of the list. So a list can be loaded with IDs 1, 4, 6, and 7, and the next ID available could be 10. These IDs 2, 3, 5, 8, and 9 were removed before and saved to file.
- Increased validation for inputs and allow aborting by typing exit
- Changed the grades data type to double and introduced rounding.
- View students now has even and odd background banding for easier searching.

# What to fix
- Comments could be more robust

- Exit is not available when removing marks and updating grades. need to add that function
- Grades could be stored in a smaller data type like float.
- Console.WriteLine($"~~~~~ {outputMessage[MessageEnum.warningFileNotFoundStudentsNotLoaded]} ~~~~~");
  Wrong message sent

If I get a chance, I will make these changes.
