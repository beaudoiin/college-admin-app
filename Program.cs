
using System.Collections.Immutable;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static Week_6_College_Admin_App.Program;
using File = System.IO.File;
namespace Week_6_College_Admin_App {
    /*
Sample Documentation
Added these notes to the top of the .cs
A bit sloppy, could use some restructuring, and a few more methods to copy reused code.

Recent Bug fixes / Features since last update
- Added labels to what menu user is in when typing in (some where not obvious)
- Fixed double EntertoContinue when search by id retunred null
- Added Personalized exit typing when adding marks, add no marks, exit says abort. add 1+ marks says done with marking
- Added exit early (typing exit) for removing marks, updating marks.
- Message on start up warrning french and spanish users to use period as decimal point and not a comma.

New Features
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

Key Features
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

What to fix
- Comments could be more robust

- Exit is not available when removing marks and updating grades. need to add that function
- Grades could be stored in a smaller data type like float.
- Console.WriteLine($"~~~~~ {outputMessage[MessageEnum.warningFileNotFoundStudentsNotLoaded]} ~~~~~");
  Wrong message sent

If I get a chance, I will make these changes.
*/
    /// <summary>
    /// A class to create an object of the Student List and IdCount.
    /// This object is to be passed to JSON for saving. (this was esier then other methods I tried)
    /// </summary>
    internal class StudentFileData {
        public List<Student>? Students {
            set; get;
        }
        public int IdCount {
            get; set;
        }
        static StudentFileData() {

        }
    }
    /// <summary>
    /// Menu-driven programming and object oriented programming
    /// create a program for a college to manipulate students information
    /// </summary>
    internal class Program {
        //Create new studentList and fileData object
        static StudentFileData fileData = new();
        static List<Student> studentList = new();
        /// <summary>
        /// Enum used to specify Remove student or View student. Used for Main menu.
        /// </summary>
        public enum RemoveOrView {
            remove,
            view
        }
        /// <summary>
        ///Provides index keys for outputMessage Dictionary. Assist in outputting the proper WriteLine messages.
        /// </summary>
        public enum MessageEnum {
            infoStartingProgram,
            savingToDisk,
            savingToDiskNoStudents,
            successStudentsSaved,
            successStudentsLoaded,
            updatedMarkSuccess,
            statusDoneAddingMarks,
            statusAbortAddingMarks,

            badInput,
            badInputStudent,
            badInputID,
            decimalFormatError,
            regexTooLong,
            regexTimedOut,
            regexBad,
            warnningWantToRemove,
            warnningWrongIndex,
            warningDeserializationFailedStudentsNotLoaded,
            warningFileNotFoundStudentsNotLoaded,
            errorFileAccessSecurityWarning,
            fileLoadOverwriteWarnning,
            fileLoadOverwriteAborted,
            fileSaveOverwriteWarnning,

            exitInput,
            exitProgram,
            exitToAbortInstruction,
            exitAddingMarksInstruction,
            exitAddingStudentsInstruction,
            exitUpdateMarks,
            exitRemoveMark,
            exitIdLookup,
            exitNameLookup,

            studentNotRemoved,
            studentNotFound,
            studentFound,
            studentAdded,
            studentHasBeenRemoved,
            studentListEmpty,
            studentNameCannotBeEmpty,
            studentsAmountAdded,
            studentSearchNoResult,
            studentSearchOneFound,
            studentSearchMultiFound,
            studentSearchMultiFoundClarifyWho,

            studentFormatFull,
            studentFormatIDName,
            markFormat,
            noMarksToDisplay,

            regexInstructions,
            regexEnterName,
            explainDecimal,
            instructAddMark,
            instructionRemoveMark,
            instructionUpdateMark,
            instructEnterFirstStudentName,
            instructEnterNextStudentName,
            instructionEnterStudentsNewName,
            instructionEnterStudentID,
            instructionEnterMarkNumber,
            instructionEnterfloatNumber,
            instructionInputMarkIndex,
            enterUpdatedMark,

            labelRemove,
            labelView,
            labelUpdate,
            labelDontUpdate,
            labelIndex,
            labelMark,
            labelOr,
            labelEnter,
            labelRemoved,
            labelChooseNumber,
            labelStudent,
            labelSelect,

            headingAdminMenu,
            menuHeaderUpdateMarks,
            menuHeaderConfirm,

            menuLabelAddStudents,
            menuLabelRemoveStudents,
            menuLabelViewAllStudents,
            menuLabelViewStudentInfo,
            menuLabelUpdateStudentInfo,
            menuLabelSaveStudentList,
            menuLabelReloadStudentList,
            menuLabelExitProgram,
            menuLabelUpdateInformation,
            menuLabelStudentViewDetials,
            menuLabelStudentUpdateName,
            menuLabelStudentUpdateMarks,
            menuLabelStudentSpecifyByID,
            menuLabelStudentSpecifyByName,
            studentSpecifyByNameMenuLabel,
            menuLabelStudentViewAllHelper,
            menuLabelUpdateMarksView,
            menuLabelUpdateMarksSpecific,
            menuLabelUpdateMarksAdd,
            menuLabelUpdateMarksRemove,
            menuLabelUpdateMarksCancel,
            menuLabelYes,
            menuLabelNo,
            menuLabelPressAnyKeyToContinue

        }
        //        Console.WriteLine( outputMessage [ MessageEnum.labelChooseNumber ] + "1, 2, 3" + outputMessage [ MessageEnum.labelOr ] + "4" );
        /*
         *      outputMessage[ MessageEnum.menuLabelUpdateInformation ]
         Console.WriteLine( outputMessage[ MessageEnum.warnningWrongIndex ] );
         */
        /// <summary>
        /// Stores the defualt outputMessages, non-comprehensive.
        /// </summary>
        public static Dictionary<MessageEnum, string> englishMessages = new() {
            { MessageEnum.exitInput, "exit" },
            { MessageEnum.badInput, "BAD INPUT! Try again!" },
            { MessageEnum.badInputStudent, $"BAD INPUT! Try again! : student is not added." },
            { MessageEnum.badInputID, "Invalid Id, insure it is an integer greater than 0." },
            { MessageEnum.exitProgram, "You are about to exit, are you sure you have saved everything and are done here?" },
            { MessageEnum.exitToAbortInstruction, "(Type exit to abort)" },
            { MessageEnum.exitAddingStudentsInstruction, "(Type exit to stop adding students)" },
            { MessageEnum.exitAddingMarksInstruction, "(Type exit to stop adding marks)" },
            { MessageEnum.studentSpecifyByNameMenuLabel, "------------ Search student by Name ------------" },
            { MessageEnum.headingAdminMenu, "Admin Menu" },
            { MessageEnum.menuLabelAddStudents, "Add students" },
            { MessageEnum.menuLabelRemoveStudents, "Remove student" },
            { MessageEnum.menuLabelViewAllStudents, "View all students info" },
            { MessageEnum.menuLabelViewStudentInfo, "View specific student info" },
            { MessageEnum.menuLabelUpdateStudentInfo, "Update information of a student" },
            { MessageEnum.menuLabelSaveStudentList, "Save userlist to file" },
            { MessageEnum.menuLabelReloadStudentList, "Reload userlist from file" },
            { MessageEnum.menuLabelUpdateInformation, "Update Information for" },
            { MessageEnum.menuLabelStudentViewDetials, "View student's details" },
            { MessageEnum.menuLabelStudentUpdateName, "Update student's name" },
            { MessageEnum.menuLabelStudentUpdateMarks, "Update student's marks" },
            { MessageEnum.menuLabelStudentSpecifyByID, "Specify by Student ID" },
            { MessageEnum.menuLabelStudentSpecifyByName, "Specify by Student name" },
            { MessageEnum.menuLabelStudentViewAllHelper, "View all students info (helper)" },
            { MessageEnum.menuLabelUpdateMarksView, "View students marks" },
            { MessageEnum.menuLabelUpdateMarksSpecific, "Update specific mark" },
            { MessageEnum.menuLabelUpdateMarksAdd, "Add new marks" },
            { MessageEnum.menuLabelUpdateMarksRemove, "Remove marks" },
            { MessageEnum.menuLabelUpdateMarksCancel, "Don't update marks" },
            { MessageEnum.menuHeaderUpdateMarks, "Update Marks" },
            { MessageEnum.menuHeaderConfirm, "Confirm" },
            { MessageEnum.menuLabelYes, "Yes" },
            { MessageEnum.menuLabelNo, "No" },
            { MessageEnum.menuLabelPressAnyKeyToContinue, "Press Any Key To Continue" },
            { MessageEnum.warningDeserializationFailedStudentsNotLoaded, "Deserialization failed - Students not loaded" },
            { MessageEnum.successStudentsSaved, "Students successfully saved" },
            { MessageEnum.warningFileNotFoundStudentsNotLoaded, "File not found - Students not loaded" },
            { MessageEnum.successStudentsLoaded, "Students successfully loaded" },
            { MessageEnum.errorFileAccessSecurityWarning, "Error accessing file: Security warning. Loading / Saving unavailable." },
            { MessageEnum.infoStartingProgram, "Starting Program" },
            { MessageEnum.menuLabelExitProgram, "Exit the program" },
            { MessageEnum.studentNotRemoved, "Student not deleted!" },
            { MessageEnum.studentNameCannotBeEmpty, "The name supplied can not be empty or only spaces!" },
            { MessageEnum.studentSearchNoResult, "Student search yielded no result!" },
            { MessageEnum.studentSearchOneFound, "One student found with name: {0}" },
            { MessageEnum.studentSearchMultiFound, "More than one student found with name: {0}" },
            { MessageEnum.studentSearchMultiFoundClarifyWho, "Please specify your student by their ID as multiple {0} has been found" },
            { MessageEnum.noMarksToDisplay, "The student doesn't have any marks to display" },
            { MessageEnum.exitUpdateMarks, "Aborting Mark updates" },
            { MessageEnum.enterUpdatedMark, "Please enter a new mark for {0}" },
            { MessageEnum.markFormat, "[Index: {0}, Mark: {1}]" },
            { MessageEnum.decimalFormatError, "Please enter a proper decimal number. (Enter a number with up to two digits after the decimal point)" },
            { MessageEnum.updatedMarkSuccess, "Student mark updated from {0} to {1} " },
            { MessageEnum.exitRemoveMark, "Aborting removing marks" },
            { MessageEnum.fileLoadOverwriteWarnning, "Warning: Current student list will be overwritten by saved file!\n"+
                "Warning: Are you sure you want to load student list from file?"},
            { MessageEnum.fileLoadOverwriteAborted, "Student list not saved (User Aborted)"},
            { MessageEnum.fileSaveOverwriteWarnning, "Warning: Current student list will be saved over a previous saved file if exists!\n"+
                "Warning: Are you sure you want to save this student list?"},
            { MessageEnum.studentFormatFull, "[ID: {0}, Name {1}, Marks: {2}]" },
            { MessageEnum.studentFormatIDName, "[ID: {0}, Name {1}]" },
            { MessageEnum.studentFound, "Student found!" },
            { MessageEnum.studentNotFound, "Student not found!" },
            { MessageEnum.savingToDisk, "Saving the following to disk!" },
            { MessageEnum.savingToDiskNoStudents, "Error, there are no students to save. Aborting save file." },
            { MessageEnum.studentListEmpty, "There are no Students enrolled at this time. Please Consider adding a student." },
            { MessageEnum.exitIdLookup, "Abort ID lookup" },
            { MessageEnum.exitNameLookup, "Abort Name lookup" },
            { MessageEnum.regexTooLong, "Name/Regex too long, please do not exceed 50 characters" },
            { MessageEnum.regexTimedOut, "Regex string takes too long to process, please simplify. Be careful when using nested quantifiers." },
            { MessageEnum.regexBad, "Bad regex string, please try again." },
            { MessageEnum.regexEnterName, "Enter Regex/name" },
            { MessageEnum.regexInstructions,
                "Enter the students name using a full name, or a portion of it.\n"+
                "Alternativley, type a Regex string that is case insensative.\n"+
                "Use (?-i:bob) to make “bob” case-sensitive, matching only lowercase bob and not Bob.\n"+
                "Regex basics: ^ start, $ end, . any character, \\d digit, \\w letter/number/underscore, \\s space, [] match one of these characters,\n"+
                "() group, | OR, * zero or more, + one or more, ? optional, {n} exact count, \\ escape special symbols."
            },
            { MessageEnum.explainDecimal, "You can input a decimal, it will be rounded to the hundredth column." },
            { MessageEnum.instructAddMark,  "Add Mark" },
            { MessageEnum.instructionRemoveMark,  "Remove Mark" },
            { MessageEnum.instructionUpdateMark,  "Update Mark" },
            { MessageEnum.instructEnterNextStudentName,  " Enter next student's name" },
            { MessageEnum.instructEnterFirstStudentName,  " Enter student's name" },
            { MessageEnum.instructionEnterStudentsNewName,  "Enter the {0}'s new name" },
            { MessageEnum.instructionEnterStudentID,  "Enter the student ID" },
            { MessageEnum.instructionEnterMarkNumber,  "Enter mark #{0}" },
            { MessageEnum.instructionEnterfloatNumber,  "Enter a number from 0 to 100 (Enter a number with up to two digits after the decimal point)" },
            { MessageEnum.instructionInputMarkIndex,  "Please select a mark by inputing it's index number" },
            { MessageEnum.studentsAmountAdded, "{0} students were added successfully!\t" },
            { MessageEnum.studentAdded, "{0} was added successfully!" },
            { MessageEnum.labelRemove, "Remove" },
            { MessageEnum.labelStudent, "Student" },
            { MessageEnum.labelRemoved, "removed" },
            { MessageEnum.labelSelect, "Select" },
            { MessageEnum.labelView, "View" },
            { MessageEnum.labelUpdate, "Update" },
            { MessageEnum.labelDontUpdate, "Don't Update" },
            { MessageEnum.labelIndex, "Index" },
            { MessageEnum.labelMark, "Mark" },
            { MessageEnum.labelOr, "or" },
            { MessageEnum.labelEnter, "Enter" },
            { MessageEnum.labelChooseNumber, "Please choose the number" },
            { MessageEnum.warnningWantToRemove, "Are you sure you want to remove" },
            { MessageEnum.warnningWrongIndex, "Wrong index, please choose from the list starting with 1 - the max number of marks" },
            { MessageEnum.studentHasBeenRemoved, "{0} has been removed!" },
            { MessageEnum.statusDoneAddingMarks, "Done adding marks!" },
            { MessageEnum.statusAbortAddingMarks, "Aborting adding marks!" },
        };
        public static Dictionary<MessageEnum, string> outputMessage = new();
        public static Dictionary<MessageEnum, string> frenchMessages = new() {
            { MessageEnum.exitInput, "quitter" },
{ MessageEnum.badInput, "ENTRÉE INVALIDE ! Veuillez réessayer !" },
{ MessageEnum.badInputStudent, "ENTRÉE INVALIDE ! Veuillez réessayer ! : l'étudiant n'a pas été ajouté." },
{ MessageEnum.badInputID, "ID invalide. Assurez-vous qu'il s'agit d'un entier supérieur à 0." },
{ MessageEnum.exitProgram, "Vous êtes sur le point de quitter. Êtes-vous sûr d'avoir sauvegardé et terminé ?" },
{ MessageEnum.exitToAbortInstruction, "(Tapez quitter pour annuler)" },
{ MessageEnum.exitAddingStudentsInstruction, "(Tapez quitter pour arrêter l'ajout d'étudiants)" },
{ MessageEnum.exitAddingMarksInstruction, "(Tapez quitter pour arrêter l'ajout des notes)" },
{ MessageEnum.studentSpecifyByNameMenuLabel, "------------ Rechercher un étudiant par nom ------------" },
{ MessageEnum.headingAdminMenu, "Menu Administrateur" },
{ MessageEnum.menuLabelAddStudents, "Ajouter des étudiants" },
{ MessageEnum.menuLabelRemoveStudents, "Supprimer un étudiant" },
{ MessageEnum.menuLabelViewAllStudents, "Voir tous les étudiants" },
{ MessageEnum.menuLabelViewStudentInfo, "Voir les informations d'un étudiant" },
{ MessageEnum.menuLabelUpdateStudentInfo, "Mettre à jour les informations d'un étudiant" },
{ MessageEnum.menuLabelSaveStudentList, "Enregistrer la liste des étudiants" },
{ MessageEnum.menuLabelReloadStudentList, "Charger la liste des étudiants" },
{ MessageEnum.menuLabelUpdateInformation, "Mettre à jour les informations pour" },
{ MessageEnum.menuLabelStudentViewDetials, "Voir les détails de l'étudiant" },
{ MessageEnum.menuLabelStudentUpdateName, "Modifier le nom de l'étudiant" },
{ MessageEnum.menuLabelStudentUpdateMarks, "Modifier les notes de l'étudiant" },
{ MessageEnum.menuLabelStudentSpecifyByID, "Spécifier par ID d'étudiant" },
{ MessageEnum.menuLabelStudentSpecifyByName, "Spécifier par nom d'étudiant" },
{ MessageEnum.menuLabelStudentViewAllHelper, "Voir tous les étudiants (assistant)" },
{ MessageEnum.menuLabelUpdateMarksView, "Voir les notes" },
{ MessageEnum.menuLabelUpdateMarksSpecific, "Modifier une note spécifique" },
{ MessageEnum.menuLabelUpdateMarksAdd, "Ajouter de nouvelles notes" },
{ MessageEnum.menuLabelUpdateMarksRemove, "Supprimer des notes" },
{ MessageEnum.menuLabelUpdateMarksCancel, "Ne pas modifier les notes" },
{ MessageEnum.menuHeaderUpdateMarks, "Mettre à jour les notes" },
{ MessageEnum.menuHeaderConfirm, "Confirmer" },
{ MessageEnum.menuLabelYes, "Oui" },
{ MessageEnum.menuLabelNo, "Non" },
{ MessageEnum.menuLabelPressAnyKeyToContinue, "Appuyez sur une touche pour continuer" },
{ MessageEnum.warningDeserializationFailedStudentsNotLoaded, "Échec de la désérialisation - Étudiants non chargés" },
{ MessageEnum.successStudentsSaved, "Étudiants enregistrés avec succès" },
{ MessageEnum.warningFileNotFoundStudentsNotLoaded, "Fichier introuvable - Étudiants non chargés" },
{ MessageEnum.successStudentsLoaded, "Étudiants chargés avec succès" },
{ MessageEnum.errorFileAccessSecurityWarning, "Erreur d'accès au fichier : Avertissement de sécurité. Chargement / Enregistrement indisponible." },
{ MessageEnum.infoStartingProgram, "Démarrage du programme" },

{ MessageEnum.menuLabelExitProgram, "Quitter le programme" },
{ MessageEnum.studentNotRemoved, "L'étudiant n'a pas été supprimé !" },

{ MessageEnum.studentNameCannotBeEmpty, "Le nom ne peut pas être vide ou contenir uniquement des espaces !" },
{ MessageEnum.studentSearchNoResult, "Aucun étudiant trouvé !" },
{ MessageEnum.studentSearchOneFound, "Un étudiant trouvé avec le nom : {0}" },
{ MessageEnum.studentSearchMultiFound, "Plus d'un étudiant trouvé avec le nom : {0}" },
{ MessageEnum.studentSearchMultiFoundClarifyWho, "Veuillez spécifier l'étudiant par ID car plusieurs {0} ont été trouvés" },
{ MessageEnum.noMarksToDisplay, "L'étudiant n'a aucune note à afficher" },
{ MessageEnum.exitUpdateMarks, "Annulation de la modification des notes" },
{ MessageEnum.enterUpdatedMark, "Veuillez entrer une nouvelle note pour {0}" },
{ MessageEnum.markFormat, "[Index : {0}, Note : {1}]" },
{ MessageEnum.decimalFormatError, "Veuillez entrer un nombre décimal valide (jusqu'à deux chiffres après la virgule)" },
{ MessageEnum.updatedMarkSuccess, "La note est passée de {0} à {1}" },
{ MessageEnum.exitRemoveMark, "Annulation de la suppression des notes" },

{ MessageEnum.fileLoadOverwriteWarnning, "Attention : La liste actuelle sera remplacée par le fichier enregistré !\nÊtes-vous sûr de vouloir charger la liste ?" },
{ MessageEnum.fileLoadOverwriteAborted, "Chargement annulé par l'utilisateur" },
{ MessageEnum.fileSaveOverwriteWarnning, "Attention : La liste actuelle écrasera un fichier existant !\nÊtes-vous sûr de vouloir enregistrer ?" },

{ MessageEnum.studentFormatFull, "[ID : {0}, Nom : {1}, Notes : {2}]" },
{ MessageEnum.studentFormatIDName, "[ID : {0}, Nom : {1}]" },
{ MessageEnum.studentFound, "Étudiant trouvé !" },
{ MessageEnum.studentNotFound, "Étudiant non trouvé !" },
{ MessageEnum.savingToDisk, "Enregistrement en cours. Appuyez sur Entrée pour continuer !" },
{ MessageEnum.savingToDiskNoStudents, "Erreur : Aucun étudiant à enregistrer. Annulation." },
{ MessageEnum.studentListEmpty, "Aucun étudiant inscrit pour le moment. Veuillez en ajouter un." },
{ MessageEnum.exitIdLookup, "Annuler la recherche par ID" },
{ MessageEnum.exitNameLookup, "Annuler la recherche par nom" },

{ MessageEnum.regexTooLong, "Nom/Regex trop long (max 50 caractères)" },
{ MessageEnum.regexTimedOut, "Expression Regex trop complexe. Veuillez simplifier." },
{ MessageEnum.regexBad, "Expression Regex invalide. Veuillez réessayer." },
{ MessageEnum.regexEnterName, "Entrez un nom ou une Regex" },
{ MessageEnum.regexInstructions,

"Entrez le nom complet de l'étudiant ou une partie de celui-ci.\n"+
"Alternativement, tapez une expression Regex insensible à la casse.\n"+
"Utilisez (?-i:bob) pour rendre “bob” sensible à la casse, ne correspondant qu'à bob en minuscules et non à Bob.\n"+
"Notions de base Regex : ^ début, $ fin, . n'importe quel caractère, \\d chiffre, \\w lettre/chiffre/souligné, \\s espace, [] correspond à un de ces caractères,\n"+
"() groupe, | OU, * zéro ou plus, + un ou plus, ? optionnel, {n} nombre exact, \\ pour échapper les symboles spéciaux."

},
{ MessageEnum.explainDecimal, "Vous pouvez entrer un nombre décimal, il sera arrondi au centième." },
{ MessageEnum.instructAddMark, "Ajouter une note" },
{ MessageEnum.instructionRemoveMark, "Supprimer une note" },
{ MessageEnum.instructionUpdateMark, "Modifier une note" },
{ MessageEnum.instructEnterNextStudentName, "Entrez le nom du prochain étudiant" },
{ MessageEnum.instructEnterFirstStudentName, "Entrez le nom de l'étudiant" },
{ MessageEnum.instructionEnterStudentsNewName, "Entrez le nouveau nom de {0}" },
{ MessageEnum.instructionEnterStudentID, "Entrez l'ID de l'étudiant" },
{ MessageEnum.instructionEnterMarkNumber, "Entrez la note #{0}" },
{ MessageEnum.instructionEnterfloatNumber, "Entrez un nombre entre 0 et 100 (jusqu'à deux décimales)" },
{ MessageEnum.instructionInputMarkIndex, "Veuillez sélectionner une note par son index" },

{ MessageEnum.studentsAmountAdded, "{0} étudiants ajoutés avec succès !" },
{ MessageEnum.studentAdded, "{0} a été ajouté avec succès !" },
{ MessageEnum.labelRemove, "Supprimer" },
{ MessageEnum.labelStudent, "Étudiant" },
{ MessageEnum.labelRemoved, "supprimé" },
{ MessageEnum.labelSelect, "Sélectionner" },
{ MessageEnum.labelView, "Voir" },
{ MessageEnum.labelUpdate, "Mettre à jour" },
{ MessageEnum.labelDontUpdate, "Ne pas mettre à jour" },
{ MessageEnum.labelIndex, "Index" },
{ MessageEnum.labelMark, "Note" },
{ MessageEnum.labelOr, "ou" },
{ MessageEnum.labelEnter, "Entrer" },
{ MessageEnum.labelChooseNumber, "Veuillez choisir le numéro" },
{ MessageEnum.warnningWantToRemove, "Êtes-vous sûr de vouloir supprimer" },
{ MessageEnum.warnningWrongIndex, "Index incorrect, veuillez choisir un numéro valide" },
{ MessageEnum.studentHasBeenRemoved, "{0} a été supprimé !" },
{ MessageEnum.statusDoneAddingMarks, "Ajout des notes terminé !" },
{ MessageEnum.statusAbortAddingMarks, "Ajout des notes annulé !" }

        };
        public static Dictionary<MessageEnum, string> spanishMessages = new() {
{ MessageEnum.exitInput, "salir" },
{ MessageEnum.badInput, "¡ENTRADA INVÁLIDA! ¡Inténtalo de nuevo!" },
{ MessageEnum.badInputStudent, "¡ENTRADA INVÁLIDA! ¡Inténtalo de nuevo! : el estudiante no fue agregado." },
{ MessageEnum.badInputID, "ID inválido. Asegúrate de que sea un número entero mayor que 0." },
{ MessageEnum.exitProgram, "Estás a punto de salir. ¿Estás seguro de que has guardado todo y terminado?" },
{ MessageEnum.exitToAbortInstruction, "(Escribe salir para cancelar)" },
{ MessageEnum.exitAddingStudentsInstruction, "(Escribe salir para detener la adición de estudiantes)" },
{ MessageEnum.exitAddingMarksInstruction, "(Escribe salir para detener la adición de notas)" },
{ MessageEnum.studentSpecifyByNameMenuLabel, "------------ Buscar estudiante por nombre ------------" },
{ MessageEnum.headingAdminMenu, "Menú Administrador" },
{ MessageEnum.menuLabelAddStudents, "Agregar estudiantes" },
{ MessageEnum.menuLabelRemoveStudents, "Eliminar estudiante" },
{ MessageEnum.menuLabelViewAllStudents, "Ver todos los estudiantes" },
{ MessageEnum.menuLabelViewStudentInfo, "Ver información de un estudiante" },
{ MessageEnum.menuLabelUpdateStudentInfo, "Actualizar información de un estudiante" },
{ MessageEnum.menuLabelSaveStudentList, "Guardar lista de estudiantes" },
{ MessageEnum.menuLabelReloadStudentList, "Cargar lista de estudiantes" },
{ MessageEnum.menuLabelUpdateInformation, "Actualizar información para" },
{ MessageEnum.menuLabelStudentViewDetials, "Ver detalles del estudiante" },
{ MessageEnum.menuLabelStudentUpdateName, "Actualizar nombre del estudiante" },
{ MessageEnum.menuLabelStudentUpdateMarks, "Actualizar notas del estudiante" },
{ MessageEnum.menuLabelStudentSpecifyByID, "Especificar por ID de estudiante" },
{ MessageEnum.menuLabelStudentSpecifyByName, "Especificar por nombre del estudiante" },
{ MessageEnum.menuLabelStudentViewAllHelper, "Ver todos los estudiantes (ayuda)" },
{ MessageEnum.menuLabelUpdateMarksView, "Ver notas" },
{ MessageEnum.menuLabelUpdateMarksSpecific, "Actualizar nota específica" },
{ MessageEnum.menuLabelUpdateMarksAdd, "Agregar nuevas notas" },
{ MessageEnum.menuLabelUpdateMarksRemove, "Eliminar notas" },
{ MessageEnum.menuLabelUpdateMarksCancel, "No actualizar notas" },
{ MessageEnum.menuHeaderUpdateMarks, "Actualizar Notas" },
{ MessageEnum.menuHeaderConfirm, "Confirmar" },
{ MessageEnum.menuLabelYes, "Sí" },
{ MessageEnum.menuLabelNo, "No" },
{ MessageEnum.menuLabelPressAnyKeyToContinue, "Presiona cualquier tecla para continuar" },
{ MessageEnum.warningDeserializationFailedStudentsNotLoaded, "Error de deserialización - Estudiantes no cargados" },
{ MessageEnum.successStudentsSaved, "Estudiantes guardados con éxito" },
{ MessageEnum.warningFileNotFoundStudentsNotLoaded, "Archivo no encontrado - Estudiantes no cargados" },
{ MessageEnum.successStudentsLoaded, "Estudiantes cargados con éxito" },
{ MessageEnum.errorFileAccessSecurityWarning, "Error al acceder al archivo: Advertencia de seguridad. Carga / Guardado no disponible." },
{ MessageEnum.infoStartingProgram, "Iniciando programa" },

{ MessageEnum.menuLabelExitProgram, "Salir del programa" },
{ MessageEnum.studentNotRemoved, "¡El estudiante no fue eliminado!" },

{ MessageEnum.studentNameCannotBeEmpty, "¡El nombre no puede estar vacío o contener solo espacios!" },
{ MessageEnum.studentSearchNoResult, "¡La búsqueda no produjo resultados!" },
{ MessageEnum.studentSearchOneFound, "Un estudiante encontrado con el nombre: {0}" },
{ MessageEnum.studentSearchMultiFound, "Más de un estudiante encontrado con el nombre: {0}" },
{ MessageEnum.studentSearchMultiFoundClarifyWho, "Por favor especifica el estudiante por su ID ya que múltiples {0} fueron encontrados" },
{ MessageEnum.noMarksToDisplay, "El estudiante no tiene notas para mostrar" },
{ MessageEnum.exitUpdateMarks, "Cancelando actualización de notas" },
{ MessageEnum.enterUpdatedMark, "Por favor ingresa una nueva nota para {0}" },
{ MessageEnum.markFormat, "[Índice: {0}, Nota: {1}]" },
{ MessageEnum.decimalFormatError, "Ingresa un número decimal válido (hasta dos cifras después del punto)" },
{ MessageEnum.updatedMarkSuccess, "La nota fue actualizada de {0} a {1}" },
{ MessageEnum.exitRemoveMark, "Cancelando eliminación de notas" },

{ MessageEnum.fileLoadOverwriteWarnning, "Advertencia: ¡La lista actual será reemplazada por el archivo guardado!\n¿Estás seguro de que deseas cargar la lista?" },
{ MessageEnum.fileLoadOverwriteAborted, "Carga cancelada por el usuario" },
{ MessageEnum.fileSaveOverwriteWarnning, "Advertencia: ¡La lista actual sobrescribirá un archivo existente!\n¿Estás seguro de que deseas guardar?" },

{ MessageEnum.studentFormatFull, "[ID: {0}, Nombre: {1}, Notas: {2}]" },
{ MessageEnum.studentFormatIDName, "[ID: {0}, Nombre: {1}]" },
{ MessageEnum.studentFound, "¡Estudiante encontrado!" },
{ MessageEnum.studentNotFound, "¡Estudiante no encontrado!" },
{ MessageEnum.savingToDisk, "Guardando en disco. ¡Presiona Enter para continuar!" },
{ MessageEnum.savingToDiskNoStudents, "Error: No hay estudiantes para guardar. Operación cancelada." },
{ MessageEnum.studentListEmpty, "No hay estudiantes inscritos actualmente. Considera agregar uno." },
{ MessageEnum.exitIdLookup, "Cancelar búsqueda por ID" },
{ MessageEnum.exitNameLookup, "Cancelar búsqueda por nombre" },

{ MessageEnum.regexTooLong, "Nombre/Regex demasiado largo (máximo 50 caracteres)" },
{ MessageEnum.regexTimedOut, "La expresión Regex tarda demasiado en procesarse. Simplifícala." },
{ MessageEnum.regexBad, "Expresión Regex inválida. Intenta nuevamente." },
{ MessageEnum.regexEnterName, "Ingresa nombre o Regex" },
{ MessageEnum.regexInstructions,

"Ingresa el nombre completo del estudiante o una parte del mismo.\n"+
"Alternativamente, escribe una expresión Regex que no distinga entre mayúsculas y minúsculas.\n"+
"Usa (?-i:bob) para hacer que “bob” distinga mayúsculas y minúsculas, coincidiendo solo con bob en minúsculas y no con Bob.\n"+
"Conceptos básicos de Regex: ^ inicio, $ fin, . cualquier carácter, \\d dígito, \\w letra/número/guion_bajo, \\s espacio, [] coincide con uno de estos caracteres,\n"+
"() grupo, | O, * cero o más, + uno o más, ? opcional, {n} cantidad exacta, \\ para escapar símbolos especiales."

},
{ MessageEnum.explainDecimal, "Puedes ingresar un número decimal, será redondeado a dos decimales." },
{ MessageEnum.instructAddMark, "Agregar nota" },
{ MessageEnum.instructionRemoveMark, "Eliminar nota" },
{ MessageEnum.instructionUpdateMark, "Actualizar nota" },
{ MessageEnum.instructEnterNextStudentName, "Ingresa el nombre del siguiente estudiante" },
{ MessageEnum.instructEnterFirstStudentName, "Ingresa el nombre del estudiante" },
{ MessageEnum.instructionEnterStudentsNewName, "Ingresa el nuevo nombre de {0}" },
{ MessageEnum.instructionEnterStudentID, "Ingresa el ID del estudiante" },
{ MessageEnum.instructionEnterMarkNumber, "Ingresa la nota #{0}" },
{ MessageEnum.instructionEnterfloatNumber, "Ingresa un número del 0 al 100 (hasta dos decimales)" },
{ MessageEnum.instructionInputMarkIndex, "Selecciona una nota ingresando su índice" },

{ MessageEnum.studentsAmountAdded, "{0} estudiantes agregados exitosamente!" },
{ MessageEnum.studentAdded, "{0} fue agregado exitosamente!" },
{ MessageEnum.labelRemove, "Eliminar" },
{ MessageEnum.labelStudent, "Estudiante" },
{ MessageEnum.labelRemoved, "eliminado" },
{ MessageEnum.labelSelect, "Seleccionar" },
{ MessageEnum.labelView, "Ver" },
{ MessageEnum.labelUpdate, "Actualizar" },
{ MessageEnum.labelDontUpdate, "No actualizar" },
{ MessageEnum.labelIndex, "Índice" },
{ MessageEnum.labelMark, "Nota" },
{ MessageEnum.labelOr, "o" },
{ MessageEnum.labelEnter, "Ingresar" },
{ MessageEnum.labelChooseNumber, "Por favor elige el número" },
{ MessageEnum.warnningWantToRemove, "¿Estás seguro de que deseas eliminar" },
{ MessageEnum.warnningWrongIndex, "Índice incorrecto, selecciona un número válido" },
{ MessageEnum.studentHasBeenRemoved, "{0} ha sido eliminado!" },
{ MessageEnum.statusDoneAddingMarks, "¡Finalizó la adición de notas!" },
{ MessageEnum.statusAbortAddingMarks, "¡Adición de notas cancelada!" },

        };
        #region /// Semantic wrapper methods for invoking ConsoleColorChange. (Human Readable and removes use of Enum)
        /// Method can be employed for ColorTextWrite and ColorTextWriteLine but I wanted to keep this more simple.
        /// <summary>
        /// Semantic wrapper method: Changes the ConsoleColor to White, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void whiteText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.White, bg );
        }
        /// <summary>
        /// Semantic wrapper method: Changes the ConsoleColor to Gray, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void grayText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.Gray, bg );
        }
        /// <summary>
        /// Changes the ConsoleColor to Green, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void greenText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.Green, bg );
        }
        /// <summary>
        /// Changes the ConsoleColor to Red, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void redText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.Red, bg );
        }
        /// <summary>
        /// Changes the ConsoleColor to Black, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void blackText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.Black, bg );
        }
        /// <summary>
        /// Changes the ConsoleColor to Cyan, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void cyanText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.Cyan, bg );
        }
        /// <summary>
        /// Changes the ConsoleColor to Yellow, Bg option, Default: Foreground color, set Bg: true, to change to Background color.
        /// </summary>
        /// <param name="bg">True: change background, False: change foregound</param>
        public static void yellowText( bool bg = false ) {
            ConsoleColorChange( ConsoleColor.Yellow, bg );
        }
        /// <summary>
        /// Changes console color based on arguments passed. Defualts to foreground, if bg: true, it will change background.
        /// </summary>
        /// <param name="color">ConsoleColor to use</param>
        /// <param name="bg">False, changes Console Foreground, True changes console background</param>
        public static void ConsoleColorChange( ConsoleColor color, bool bg ) {
            if ( bg == true )
                Console.BackgroundColor = color;
            else
                Console.ForegroundColor = color;
        }
        #endregion
        //Set the default file name for saving serialized JSON
        static string filename = @"studentprogtemp.dat";
        /// <summary>
        /// Entry point and logic for the main menu.
        /// </summary>
        /// <param name="args"></param>
        static void Main( string [ ] args ) {
            redText();
            Console.WriteLine( "Veuillez utiliser le point (.) comme séparateur décimal, comme en anglais.\nUtilice el punto (.) como separador decimal, como en inglés." );

            string lang;
            while ( true ) {
                whiteText();
                lang = ChooseLanguage();
                switch ( lang ) {
                    case "fr":
                        outputMessage = frenchMessages;
                        break;

                    case "es":
                        outputMessage = spanishMessages;
                        break;

                    case "en":
                        outputMessage = englishMessages;
                        break;
                    default:

                        Console.Clear();
                        redText();
                        Console.WriteLine( "Bad Input! Enter only 2 letters corresponding to the list    |    Entrée invalide ! Entrez seulement 2 lettres correspondant à la liste    |    ¡Entrada inválida! Introduzca solo 2 letras correspondientes a la lista" );
                        continue;


                }
                break;
            }
            Console.CursorVisible = false;
            #region ///Validate and set path variable and load studentList from disk
            string? savePath = null;
            try {
                savePath = Path.GetTempPath();
                filename = Path.Combine( savePath, filename );
            } catch ( SecurityException ) {
                Console.WriteLine( outputMessage [ MessageEnum.errorFileAccessSecurityWarning ] );
                EnterToContinue();
            }
            LoadUserList( true );
            #endregion
            #region ///Thread Sleep "Program Starting" animation
            whiteText();
            Thread.Sleep( 500 );
            string msg = outputMessage [ MessageEnum.infoStartingProgram ];
            string msg2 = "...";
            foreach ( char c in msg ) {
                Thread.Sleep( 30 );
                Console.Write( c );
            }
            foreach ( char c in msg2 ) {
                Thread.Sleep( 100 );
                Console.Write( c );
            }
            Thread.Sleep( 400 );
            Console.CursorVisible = true;
            Console.Clear();
            //clear the buffer of keys pressed when user was waiting due to Thread.Sleep
            while ( Console.KeyAvailable ) {
                Console.ReadKey( true );
            }

            #endregion
            //Main Menu logic
            while ( true ) {
                string? choice = MainMenu();//Get user input and clear the screen.
                switch ( choice ) {
                    case "1":
                        AddStudent();
                        break;
                    //Remove specific student
                    case "2":
                        RemoveViewStudent( RemoveOrView.remove ); //Similar function, specify Removing.
                        break;
                    //View all students, leaves on screen until enter is pressed.
                    case "3":
                        ViewAllStudents();
                        EnterToContinue( ClearAfter: true );
                        break;
                    //View sSpecific student
                    case "4":
                        RemoveViewStudent( RemoveOrView.view ); //Similar function, specify Viewing.
                        break;
                    case "5": UpdateStudentInfo(); break;
                    case "6": WriteUserList(); break; // Saves the users data to a file
                    case "7": LoadUserList(); break; // Load the users data from a file
                    case "8":
                        redText();
                        Console.WriteLine( outputMessage [ MessageEnum.exitProgram ] );
                        int choiceExit = Confirm();
                        if ( choiceExit == 1 )
                            Environment.Exit( 0 );
                        break;
                    default:
                        Console.Clear();
                        if ( choice != "" )
                            Console.WriteLine( outputMessage [ MessageEnum.badInput ] );
                        break;
                }
            }
        }
        /// <summary>
        /// Produced quickly by chatGPT, and so was the french and english translatinons. This was by chatGPT because im in a hurry!
        /// </summary>
        /// <returns>language is 2 letter stirng</returns>
        private static string ChooseLanguage() {

            whiteText();

            Console.WriteLine( "Choose a language | Choisissez une langue | Elige un idioma" );
            Console.WriteLine( "\ten - English" );
            Console.WriteLine( "\tfr - Français" );
            Console.WriteLine( "\tes - Español" );
            Console.WriteLine();

            Console.Write( "Type in 2 letters | Tapez 2 lettres | Escribe 2 letras : " );
            yellowText();

            string input = Console.ReadLine()!.Trim().ToLower();

            return input;
        }
        /// <summary>
        /// Adds a student, requires their Name. You can add no marks, or multiple marks.
        /// </summary>
        private static void AddStudent() {
            Console.Clear();
            int addedCount = 0;
            ///Validate Name input
            while ( true ) {
                grayText();
                Console.Write( outputMessage [ MessageEnum.exitAddingStudentsInstruction ] );
                whiteText();
                if ( addedCount > 0 )
                    Console.Write( $"{outputMessage [ MessageEnum.instructEnterNextStudentName ]} : " );
                else
                    Console.Write( $"{outputMessage [ MessageEnum.instructEnterFirstStudentName ]} : " );
                yellowText();
                string name = Console.ReadLine()!;
                //Display exit message when finished
                if ( name == outputMessage [ MessageEnum.exitInput ] ) {
                    greenText();
                    Console.WriteLine( outputMessage [ MessageEnum.studentsAmountAdded ], addedCount );
                    EnterToContinue( ClearAfter: true );
                    break;
                }
                if ( string.IsNullOrWhiteSpace( name ) ) {
                    redText();
                    Console.WriteLine( outputMessage [ MessageEnum.badInputStudent ] );
                    greenText();
                    continue;
                }
                greenText();
                //Add Marks to new student from AddMarks Method.
                addedCount++;
                studentList.Add( new Student( name, AddMarks() ) );
                Console.Clear();
                Console.WriteLine( outputMessage [ MessageEnum.studentAdded ], name );
            }
        }
        /// <summary>
        /// Remove or View a specific student. Set the enum type to specify. (Methods almost identical)
        /// </summary>
        /// <param name="type">Enum "Remove" or "View" strictly specifies action</param>
        private static void RemoveViewStudent( RemoveOrView type ) {
            string label = ""; // Formating menu label
            Console.Clear();
            if ( !CheckStudentsExist() ) {
                EnterToContinue();
                Console.Clear();
                return; // If there are no students, print a message and return.
            }
            //Display menu for various methods to get the student
            switch ( type ) {
                case RemoveOrView.remove:
                    label = outputMessage [ MessageEnum.labelRemove ];
                    break;
                case RemoveOrView.view:
                    label = outputMessage [ MessageEnum.labelView ];
                    break;
            }
            //Opens the get student method to allow for different ways of selecting a student. (Tailored menu name with label)
            Student? student = getStudent( label );
            //Remove or View student logic
            if ( student != null ) {
                switch ( type ) {
                    //Remove has some extra confirmation due to the destructive nature of removing
                    case RemoveOrView.remove:
                        Console.WriteLine( $"{outputMessage [ MessageEnum.warnningWantToRemove ]} : {outputMessage [ MessageEnum.studentFormatIDName ]}?", student.Id, student.Name );
                        int choice = Confirm();
                        Console.Clear();
                        if ( choice == 1 ) {
                            redText();
                            //This one needs explination, I am loading from dictionary a string with a placeholder for student object, but student object is formattings a string with variables
                            //similar to "[ID: {0}, Name {1}, Marks: {2}]", so I plug those into "{0} has been removed!"
                            //Now we cabn get something like [ID: 12, Name Eric, Marks: [2,10,99, Avg 12]] has been removed!"
                            //PURPOSE: I do this incase another language needs to construct the sentence differently, a new language dictionary may put {0} in a different spot
                            //              depending on the grammer used.
                            Console.WriteLine( outputMessage [ MessageEnum.studentHasBeenRemoved ], string.Format( outputMessage [ MessageEnum.studentFormatIDName ], student.Id, student.Name ) );
                            studentList.Remove( student );
                        } else {
                            greenText();
                            Console.WriteLine( outputMessage [ MessageEnum.studentNotRemoved ] );
                        }
                        break;
                    case RemoveOrView.view:
                        Console.Clear();
                        Console.WriteLine( student );
                        break;
                }
            }
        }
        /// <summary>
        /// Write all students to screen with a banded bacgrkoundcolor table
        /// </summary>
        private static void ViewAllStudents() {
            //Clear previous menu
            Console.Clear();
            //If there are no students, print a message
            if ( !CheckStudentsExist() ) return;
            bool odd = true;
            foreach ( var item in studentList ) {
                //Switches colors to allow for banding rows formatting
                if ( odd ) {
                    blackText();
                    whiteText( bg: true );
                } else {
                    whiteText();
                    blackText( bg: true );
                }
                Console.Write( item );
                greenText();
                blackText( bg: true );
                Console.WriteLine( "" );
                odd = !odd;
            }
        }
        private static void UpdateStudentInfo() {
            Console.Clear();
            Student? student = getStudent( outputMessage [ MessageEnum.labelUpdate ] ); // update is for the menu label

            //Exit this menu , error thrown
            if ( student is null ) {
                EnterToContinue();
                Console.Clear();
                return;
            }
            //Update student info menu logic
            bool updateMenu = true;
            while ( updateMenu ) {
                string? choice = UpdateStudentDetailsMenu( student );
                switch ( choice ) {

                    //View students details
                    case "1":
                        Console.Clear();
                        cyanText();
                        Console.WriteLine( student.ToString() );
                        break;

                    //Update Students name
                    case "2":
                        Console.Clear();
                        string name;

                        //Get and Validate name
                        while ( true ) {
                            whiteText();
                            Console.Write( $"{outputMessage [ MessageEnum.instructionEnterStudentsNewName ]} : ", student.Name );
                            yellowText();
                            name = Console.ReadLine()!;
                            Console.ForegroundColor = ConsoleColor.Green;
                            if ( !string.IsNullOrWhiteSpace( name ) ) break;
                            redText();
                            Console.WriteLine( outputMessage [ MessageEnum.studentNameCannotBeEmpty ] );
                        }
                        student.Name = name;
                        break;

                    //Update marks
                    case "3":
                        Console.Clear();
                        //Student has no marks, ask to add marks
                        if ( student.Marks == null || student.Marks.Length == 0 ) {
                            Console.WriteLine( outputMessage [ MessageEnum.noMarksToDisplay ] );
                            student.Marks = AddMarks();
                            Console.Clear();
                            break;
                        }
                        //Update Marks Menu
                        bool marksMenu = true;
                        int index;
                        //Marks available, open NESTED menu for updating marks 
                        while ( marksMenu ) {
                            //Method to display marks using a loop to format with index and mark.
                            //Used for refrence when selecting marks to update.
                            void DisplayMarks() {
                                for ( int i = 0; i < student.Marks.Length; i++ ) {
                                    cyanText();
                                    Console.Write( $"[{outputMessage [ MessageEnum.labelIndex ]}: {i + 1}, " );
                                    greenText();
                                    Console.Write( $"{outputMessage [ MessageEnum.labelMark ]}: {student.Marks [ i ]}]" );
                                    if ( i != student.Marks.Length - 1 )
                                        Console.Write( ", " );
                                    else
                                        Console.WriteLine();
                                }
                            }
                            //Various options for editing grades, Display (in switch) and logic
                            string? choice2 = UpdateGradesMenu( student );
                            switch ( choice2 ) {

                                //Display Marks
                                case "1":
                                    Console.Clear();
                                    if ( student.Marks.Length > 0 )
                                        DisplayMarks();
                                    else
                                        Console.WriteLine( outputMessage [ MessageEnum.noMarksToDisplay ] );
                                    break;

                                //Update specific mark
                                case "2":
                                    Console.Clear();
                                    DisplayMarks();
                                    index = SelectStudentMark( student );
                                    if ( index == 0 ) {
                                        redText();
                                        Console.WriteLine( outputMessage [ MessageEnum.exitUpdateMarks ] );
                                        greenText();
                                        EnterToContinue( ClearAfter: true );
                                        break;
                                    }
                                    //save the updated mark, old mark, and a concatenated string.
                                    float updatedMark;
                                    float oldMark = student.Marks [ index - 1 ];
                                    //Reads from dictionary and plus in the variables.
                                    string oldMarkString = string.Format( outputMessage [ MessageEnum.markFormat ], index, oldMark );
                                    whiteText();
                                    //Mark Selected
                                    Console.Write( $"{outputMessage [ MessageEnum.enterUpdatedMark ]} : ", oldMarkString );
                                    //Validate Mark
                                    while ( true ) {
                                        yellowText();
                                        if ( float.TryParse( Console.ReadLine(), out updatedMark ) && updatedMark >= 0 && updatedMark <= 100 ) {
                                            updatedMark = ( float ) Math.Round( updatedMark, 2 );
                                            student.Marks [ index - 1 ] = updatedMark;
                                            greenText();
                                            break;
                                        }
                                        redText();
                                        Console.WriteLine( outputMessage [ MessageEnum.decimalFormatError ] );
                                    }
                                    Console.WriteLine( outputMessage [ MessageEnum.updatedMarkSuccess ], oldMark, updatedMark );
                                    EnterToContinue( ClearAfter: true );
                                    break;

                                //Add Marks
                                case "3":
                                    Console.Clear();
                                    student.Marks = student.Marks.Concat( AddMarks() ).ToArray();
                                    Console.Clear();
                                    break;

                                //Remove Marks
                                case "4":
                                    Console.Clear();
                                    if ( student.Marks.Length == 0 ) {
                                        Console.WriteLine( outputMessage [ MessageEnum.noMarksToDisplay ] );
                                        EnterToContinue( ClearAfter: true );
                                        break;
                                    } else {
                                        DisplayMarks();
                                        redText();
                                        Console.Write( $"{outputMessage [ MessageEnum.instructionRemoveMark ]} : " );
                                        whiteText();
                                        index = SelectStudentMark( student );
                                        if ( index == 0 ) {
                                            redText();
                                            Console.WriteLine( outputMessage [ MessageEnum.exitRemoveMark ] );
                                            greenText();
                                            EnterToContinue( ClearAfter: true );
                                            break;

                                        }
                                        redText();
                                        Console.WriteLine( $"{outputMessage [ MessageEnum.markFormat ]} {outputMessage [ MessageEnum.labelRemoved ]}!", index, student.Marks [ index - 1 ] );
                                        //Makes it Immuatable (unchange able) but gives the handy remove function
                                        //Remove creates a copy with the index removed, and we cast it back to a regular fixed array.
                                        student.Marks = student.Marks.ToImmutableArray().RemoveAt( index - 1 ).ToArray();
                                        whiteText();
                                    }
                                    EnterToContinue( ClearAfter: true );
                                    break;

                                //Exit menu
                                case "5":
                                    marksMenu = false;
                                    break;

                                //Bad input
                                default:
                                    Console.Clear();
                                    if ( choice2 != "" )
                                        Console.WriteLine( outputMessage [ MessageEnum.badInput ] );
                                    break;
                            }
                        }
                        Console.Clear();
                        break;

                    //Exit update student info menu
                    case "4":
                        updateMenu = false;
                        Console.Clear();
                        break;

                    //Bad input
                    default:
                        Console.Clear();
                        if ( choice != "" ) {
                            Console.WriteLine( outputMessage [ MessageEnum.badInput ] );
                        }
                        break;
                }
            }

        }
        private static void LoadUserList( bool startup = false ) {
            if ( startup ) { loadFile(); return; }
            redText();
            Console.WriteLine( $"{outputMessage [ MessageEnum.fileLoadOverwriteWarnning ]} : " );
            switch ( Confirm() ) {
                // Chose to load file
                case 1:
                    loadFile();
                    return;
                // Chose not to load file
                case 2:
                    return;
            }
            //Restores student list and the unique idCount to prevent duplicate student ids.
            static void loadFile() {
                StudentFileData? studentListLoad = SecureFile.Load<StudentFileData>( filename );
                if ( studentListLoad != null ) {
                    studentList = studentListLoad.Students!;
                    Student.s_IdCount = studentListLoad.IdCount;
                }
                EnterToContinue();
                Console.Clear();
            }

        }
        /// <summary>
        /// Load User list to a file if there is users in the current memory. Prompts for conformation
        /// </summary>
        private static void WriteUserList() {
            Console.Clear();

            Console.Clear();
            if ( !CheckStudentsExist() ) {
                EnterToContinue();
                Console.Clear();
                return; // If there are no students, print a message and return.
            }

            while ( true ) {
                redText();
                Console.WriteLine( $"{outputMessage [ MessageEnum.fileSaveOverwriteWarnning ]} : " );
                switch ( Confirm() ) {
                    case 1:
                        //Writes the student by specified format in outputMessage dictionary
                        foreach ( var s in studentList )
                            Console.WriteLine( outputMessage [ MessageEnum.studentFormatFull ], s.Id, s.Name, s.Marks );
                        Console.WriteLine( outputMessage [ MessageEnum.savingToDisk ] );
                        EnterToContinue();
                        //Update fileData object to contain student list and the IdCount;
                        fileData.Students = studentList;
                        fileData.IdCount = Student.s_IdCount;
                        SecureFile.Save( filename, fileData );
                        Console.Clear();
                        return;

                    case 2:

                        Console.WriteLine( outputMessage [ MessageEnum.fileLoadOverwriteAborted ] );
                        EnterToContinue();
                        Console.Clear();
                        return;
                }
            }
        }
        /// <summary>
        /// Gets Student by various methods. Specify by ID or Name.
        /// </summary>
        /// <param name="task">A label specifying the reason that gets appened to the menu title.</param>
        /// <returns>Instance of Student or null</returns>
        private static Student? getStudent( string task = "" ) {
            //Can't get Students if none are created in memory first.
            if ( !CheckStudentsExist() ) return null;
            Student? student = null; // Update to student when found.
            //Student Select Menu
            while ( student is null ) {
                string choice = ChooseStudentMenu( task );
                switch ( choice ) {

                    //Get student by ID
                    case "1":
                        student = SpecifyStudentByID( studentList ); //This has its own null regection
                        Console.Clear();
                        continue;

                    //Get student by Name
                    case "2":
                        student = SpecifyStudentByName();
                        continue;

                    //no student returned by name
                    case "3":
                        //Helper option to view all student again
                        Console.Clear();
                        ViewAllStudents();
                        //Exit back to main menu
                        break;

                    //Exit selected
                    case "4":
                        Console.Clear();
                        return null;

                    //Wrong input
                    default:
                        Console.Clear();
                        if ( choice != "" ) {
                            Console.WriteLine( $"{outputMessage [ MessageEnum.labelChooseNumber ]} 1, 2, 3 {outputMessage [ MessageEnum.labelOr ]} 4" );
                        }
                        break;
                }
                //Loops to start if no student found (null)
            }
            return student;
        }
        //Method Aproaches to find users.
        /// <summary>
        /// Chose the student by inputing their ID. You may want to call recursivley, and pass narrowed down copies of the list.
        /// </summary>
        /// <param name="studentListSearch">Pass the list to search for students.</param>
        /// <returns></returns>
        private static Student? SpecifyStudentByID( List<Student> studentListSearch ) {
            int id;
            string idCapture;
            //Get student id input validated
            while ( true ) {
                whiteText();
                Console.Write( $"{outputMessage [ MessageEnum.exitToAbortInstruction ]} {outputMessage [ MessageEnum.instructionEnterStudentID ]} " );
                yellowText();
                idCapture = Console.ReadLine()!;
                //Proper ID index entered, Silently checks (0 not allowed)
                if ( int.TryParse( idCapture, out id ) && id > 0 )
                    break;
                //Specify user exited
                if ( idCapture == outputMessage [ MessageEnum.exitInput ] )
                    break;
                redText();
                Console.WriteLine( outputMessage [ MessageEnum.badInputID ] );
            }

            //Exit because user typed nothing (on purpose)
            greenText();
            if ( idCapture != outputMessage [ MessageEnum.exitInput ] ) {
                foreach ( var student in studentListSearch ) {
                    if ( student.Id == id ) {
                        Console.WriteLine( outputMessage [ MessageEnum.studentFound ] );
                        return student;
                    }
                }
                redText();
                Console.WriteLine( outputMessage [ MessageEnum.studentNotFound ] );
            } else {
                redText();
                Console.WriteLine( outputMessage [ MessageEnum.exitIdLookup ] );
            }
            greenText();
            EnterToContinue( ClearAfter: true );
            return null;
        }
        /// <summary>
        /// Chose the student by name search, supports partial name match and user entering Regex string.
        /// This will narrow down the list and if there is more then one, print new list and invoke SpecifyStudentByID()
        /// </summary>
        /// <returns></returns>
        private static Student? SpecifyStudentByName() {
            Console.Clear();
            whiteText();
            Console.WriteLine( outputMessage [ MessageEnum.studentSpecifyByNameMenuLabel ] );
            string name;
            List<Student> studentMatches = studentList;
            //Request and Validate name, below loop is to improve readability. (Changing color for each bullet point)
            //reads from a dictionary and splits a single string with \n to individual stirngs, allowing foreach to format with bullet point
            string [ ] instructions = outputMessage [ MessageEnum.regexInstructions ].Split( '\n' );
            string bullet = "\t» ";
            foreach ( var msg in instructions ) {
                cyanText();
                Console.Write( bullet );
                greenText();
                Console.WriteLine( msg );
            }
            whiteText();
            //Main loop, Loops if regex has exceptions (timeout exception and argument exception)
            while ( true ) {
                //Loop if regex is too long or empty
                while ( true ) {
                    whiteText();
                    Console.Write( $"{outputMessage [ MessageEnum.exitToAbortInstruction ]}{outputMessage [ MessageEnum.regexEnterName ]} : " );
                    yellowText();
                    name = Console.ReadLine()!;
                    if ( name.Length > 50 ) {
                        redText();
                        Console.WriteLine( outputMessage [ MessageEnum.regexTooLong ] );
                        continue;
                    }
                    greenText();
                    if ( !string.IsNullOrWhiteSpace( name ) || name == outputMessage [ MessageEnum.exitInput ] )
                        break;
                    redText();
                    Console.WriteLine( outputMessage [ MessageEnum.studentNameCannotBeEmpty ] );
                }
                //Exit because user typed nothing (on purpose)
                if ( name == outputMessage [ MessageEnum.exitInput ] ) {
                    redText();
                    Console.WriteLine( outputMessage [ MessageEnum.exitNameLookup ] );
                    greenText();
                    EnterToContinue( ClearAfter: true );
                    return null;
                }
                String nameRegPatternLoose = $@"{name}"; //@ avoids escaping "\" with "\\"
                String nameRegPatternExact = $@"^{name}$"; //@ avoids escaping "\" with "\\"

                //Loop for Through up to date studentList
                try {
                    Regex nameReg = new( nameRegPatternLoose, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds( 500 ) );
                    studentMatches = studentList.Where( item => nameReg.IsMatch( item.Name ) ).ToList();
                } catch ( RegexMatchTimeoutException ) {
                    redText();
                    Console.WriteLine( outputMessage [ MessageEnum.regexTimedOut ] );
                    continue;
                } catch ( ArgumentException ) {
                    redText();
                    Console.WriteLine( outputMessage [ MessageEnum.regexBad ] );
                    continue;
                }
                ///Check multiples, update list or Return Student if one
                switch ( studentMatches.Count ) {
                    case 0:
                        redText();
                        Console.WriteLine( outputMessage [ MessageEnum.studentSearchNoResult ] );
                        greenText();
                        EnterToContinue( ClearAfter: true );
                        return null;
                    case 1:
                        Console.WriteLine( outputMessage [ MessageEnum.studentSearchOneFound ], name );
                        EnterToContinue( ClearAfter: true );
                        return studentMatches [ 0 ];
                    default:
                        Console.WriteLine( outputMessage [ MessageEnum.studentSearchMultiFound ], name );
                        foreach ( var student in studentMatches )
                            Console.WriteLine( student );
                        Console.WriteLine( outputMessage [ MessageEnum.studentSearchMultiFoundClarifyWho ], name );
                        Student return_student = SpecifyStudentByID( studentList )!;
                        if ( return_student != null )
                            EnterToContinue();
                        return return_student;
                }
            }
        }
        //Required methos dealing with updating user details (called by UpdateStudentInfo())
        /// <summary>
        /// Adds marks to the students specified by the user.
        /// </summary>
        /// <returns>float array of new concatenated marks</returns>
        private static float [ ] AddMarks() {
            int addedCount = 1;
            List<float> marks = new();
            Console.WriteLine( outputMessage [ MessageEnum.explainDecimal ] );
            while ( true ) {
                whiteText();
                greenText();
                Console.Write( $"{outputMessage [ MessageEnum.instructAddMark ]} : " );
                whiteText();
                Console.Write( outputMessage [ MessageEnum.exitAddingMarksInstruction ] );
                Console.Write( $" {outputMessage [ MessageEnum.instructionEnterMarkNumber ]} : ", addedCount );

                yellowText();
                string mark = Console.ReadLine()!;
                if ( mark == outputMessage [ MessageEnum.exitInput ] ) {

                    if ( addedCount > 1 ) {
                        greenText();
                        Console.Write( outputMessage [ MessageEnum.statusDoneAddingMarks ] + "\t" );
                    } else {
                        redText();
                        Console.Write( outputMessage [ MessageEnum.statusAbortAddingMarks ] + "\t" );
                    }
                    greenText();
                    EnterToContinue();
                    break;
                }
                if ( float.TryParse( mark, out float e ) && e >= 0 && e <= 100 ) {
                    addedCount++;
                    e = ( float ) Math.Round( e, 2 );
                    marks.Add( e );
                    continue;
                }
                redText();
                Console.WriteLine( outputMessage [ MessageEnum.instructionEnterfloatNumber ] );
            }
            greenText();
            return marks.ToArray();
        }
        /// <summary>
        /// Allow user to select a students mark for further updaing by method invoking this.
        /// </summary>
        /// <param name="student">Instance of student for selecting their marks</param>
        /// <returns>Index chosen for student</returns>
        private static int SelectStudentMark( Student student ) {
            while ( true ) {
                greenText();
                Console.Write( $"{outputMessage [ MessageEnum.instructionUpdateMark ]} : " );
                whiteText();
                Console.Write( $"{outputMessage [ MessageEnum.exitToAbortInstruction ]} {outputMessage [ MessageEnum.instructionInputMarkIndex ]} : " );
                yellowText();
                string markSpecifiedIndex = Console.ReadLine()!;
                if ( markSpecifiedIndex == outputMessage [ MessageEnum.exitInput ] )
                    return 0;

                if ( int.TryParse( markSpecifiedIndex, out int index ) && index > 0 && index < student.Marks.Length + 1 ) {
                    greenText();
                    return index;
                } else {
                    redText();
                    Console.WriteLine( outputMessage [ MessageEnum.warnningWrongIndex ] );
                }
            }
        }
        //Menu Selection Lists
        /// <summary>
        /// Text menu for the main manu
        /// </summary>
        /// <returns>Choice</returns>
        private static string MainMenu() {
            whiteText();
            Console.WriteLine( $"------------ {outputMessage [ MessageEnum.headingAdminMenu ]} ------------" );
            greenText();
            Console.WriteLine( "\t1 - " + outputMessage [ MessageEnum.menuLabelAddStudents ] );
            Console.WriteLine( "\t2 - " + outputMessage [ MessageEnum.menuLabelRemoveStudents ] );
            Console.WriteLine( "\t3 - " + outputMessage [ MessageEnum.menuLabelViewAllStudents ] );
            Console.WriteLine( "\t4 - " + outputMessage [ MessageEnum.menuLabelViewStudentInfo ] );
            Console.WriteLine( "\t5 - " + outputMessage [ MessageEnum.menuLabelUpdateStudentInfo ] );
            Console.WriteLine( "\t6 - " + outputMessage [ MessageEnum.menuLabelSaveStudentList ] );
            Console.WriteLine( "\t7 - " + outputMessage [ MessageEnum.menuLabelReloadStudentList ] );
            Console.WriteLine( "\t8 - " + outputMessage [ MessageEnum.menuLabelExitProgram ] );
            whiteText();
            Console.Write( $"{outputMessage [ MessageEnum.labelEnter ]} 1, 2, 3, 4 , 5, 6, 7 {outputMessage [ MessageEnum.labelOr ]} 8 : " );
            yellowText();
            string return_str = Console.ReadLine()!;
            greenText();
            return return_str;
        }
        /// <summary>
        /// Text menu for updating the students details.
        /// </summary>
        /// <param name="student">Instance of a student</param>
        /// <returns>Choice</returns>
        private static string UpdateStudentDetailsMenu( Student student ) {
            if ( !CheckStudentsExist() ) {
                EnterToContinue();
                Console.Clear();
                return "4";
            }
            whiteText();
            Console.WriteLine( $"------------ {outputMessage [ MessageEnum.menuLabelUpdateInformation ]} {student.Name} ------------" );
            greenText();
            Console.WriteLine( "\t1 - " + outputMessage [ MessageEnum.menuLabelStudentViewDetials ] );
            Console.WriteLine( "\t2 - " + outputMessage [ MessageEnum.menuLabelStudentUpdateName ] );
            Console.WriteLine( "\t3 - " + outputMessage [ MessageEnum.menuLabelStudentUpdateMarks ] );
            Console.WriteLine( "\t4 - " + outputMessage [ MessageEnum.labelDontUpdate ] );
            whiteText();
            Console.Write( $"{outputMessage [ MessageEnum.labelEnter ]} 1, 2, 3, {outputMessage [ MessageEnum.labelOr ]} 4 : " );
            yellowText();
            string return_str = Console.ReadLine()!;
            greenText();
            return return_str;
        }
        /// <summary>
        /// Text menu for choosing student.
        /// </summary>
        /// <param name="task">You can prepent a word the the menu ---- [task] Student, defualt is "Select"</param>
        /// <returns>Choice</returns>
        private static string ChooseStudentMenu( string task ) {
            //format spacing depending if user passes "Remove" or "View", etc..
            if ( task == "" )
                task = outputMessage [ MessageEnum.labelSelect ];
            whiteText();
            Console.WriteLine( $"------------ {task} {outputMessage [ MessageEnum.labelStudent ]} ------------" );
            greenText();
            Console.WriteLine( "\t1 - " + outputMessage [ MessageEnum.menuLabelStudentSpecifyByID ] );
            Console.WriteLine( "\t2 - " + outputMessage [ MessageEnum.menuLabelStudentSpecifyByName ] );
            Console.WriteLine( "\t3 - " + outputMessage [ MessageEnum.menuLabelStudentViewAllHelper ] );
            Console.WriteLine( "\t4 - " + outputMessage [ MessageEnum.labelDontUpdate ] );
            whiteText();
            Console.Write( $"{outputMessage [ MessageEnum.labelEnter ]} 1, 2, 3 {outputMessage [ MessageEnum.labelOr ]} 4 : " );
            yellowText();
            string return_str = Console.ReadLine()!;
            greenText();
            return return_str;
        }
        /// <summary>
        /// Text menu for updating marks.
        /// </summary>
        /// <param name="student">Instance of student updating marks for.</param>
        /// <returns>Users choice</returns>
        private static string? UpdateGradesMenu( Student student ) {
            whiteText();
            Console.WriteLine( $"------------ {outputMessage [ MessageEnum.menuHeaderUpdateMarks ]} {student.Name} ------------" );
            greenText();
            Console.WriteLine( $"\t1 - {outputMessage [ MessageEnum.menuLabelUpdateMarksView ]}" );
            Console.WriteLine( $"\t2 - {outputMessage [ MessageEnum.menuLabelUpdateMarksSpecific ]}" );
            Console.WriteLine( $"\t3 - {outputMessage [ MessageEnum.menuLabelUpdateMarksAdd ]}" );
            Console.WriteLine( $"\t4 - {outputMessage [ MessageEnum.menuLabelUpdateMarksRemove ]}" );
            Console.WriteLine( $"\t5 - {outputMessage [ MessageEnum.menuLabelUpdateMarksCancel ]}" );
            whiteText();
            Console.Write( $"{outputMessage [ MessageEnum.labelEnter ]} 1, 2, 3, 4, {outputMessage [ MessageEnum.labelOr ]} 5 : " );
            yellowText();
            string return_str = Console.ReadLine()!;
            greenText();
            return return_str;
        }
        //Extra method
        /// <summary>
        /// Returns true if studentList has students, Prints a no student message and returns false
        /// </summary>
        /// <returns>true or false if there are students inside studentList</returns>
        private static bool CheckStudentsExist() {
            if ( studentList.Count > 0 )
                return true;
            Console.WriteLine( outputMessage [ MessageEnum.studentListEmpty ] );
            return false;
        }
        /// <summary>
        /// Get user input for Yes/No. Could be returned as a  bool instead
        /// </summary>
        /// <returns>Users choice as int</returns>
        private static int Confirm() {
            int choice;
            redText();
            Console.WriteLine( $"------------ {outputMessage [ MessageEnum.menuHeaderConfirm ]} ------------" );
            Console.WriteLine( $"\t1 - {outputMessage [ MessageEnum.menuLabelYes ]}" );
            Console.WriteLine( $"\t2 - {outputMessage [ MessageEnum.menuLabelNo ]}" );
            while ( true ) {
                whiteText();
                Console.Write( $"{outputMessage [ MessageEnum.labelEnter ]} 1 {outputMessage [ MessageEnum.labelOr ]} 2: " );
                yellowText();
                if ( int.TryParse( Console.ReadLine(), out choice ) && ( choice == 1 || choice == 2 ) ) {
                    greenText();
                    return choice;
                }

            }

        }
        public static void EnterToContinue( bool ClearAfter = false ) {
            Console.WriteLine( outputMessage [ MessageEnum.menuLabelPressAnyKeyToContinue ] );
            Console.CursorVisible = false;
            Console.ReadKey( true );
            Console.CursorVisible = true;
            if ( ClearAfter ) {
                Console.Clear();
            }
        }
    }
    [Serializable]
    internal class Student {
        // why do we have an ID? --> Primary key for fetching faster. Has to be unique.
        public static int s_IdCount { get; set; } = 0;
        /*  public int id {
              get;
          } /// attribute < removed because JSON case sensitivity is mixing it up with Id*/
        public int Id {
            get; private set;
        } // property
        public string Name {
            get; set;
        }
        public float [ ] Marks {
            get; set;
        }
        // make id autoincrementally and set that automatically
        [JsonConstructor]
        public Student( int id, string name, float [ ] marks ) {
            Id = id;
            Name = name;
            Marks = marks;
        }
        public Student( string name, float [ ] marks ) {
            Id = ++s_IdCount;
            Name = name;
            Marks = marks;
        }
        public float Avg() {
            float add = 0F;
            foreach ( var mark in Marks ) {
                add += mark;
            }
            //Currently this issue doesnt happen due to ToString checking, but this may be used elsewhere if expanding.
            if ( Marks.Length > 0 ) {
                return ( float ) Math.Round( add / Marks.Length, 2 );
            } else {
                return 0F; // return zero due to devide by zero
            }

        }
        public override string? ToString() {
            string studentString = $"Id: {Id}, Name: {Name}";
            studentString = Marks.Length > 0
                ? string.Concat( studentString, $", Marks: [{string.Join( ", ", Marks )}]" +
                $", Average: {Avg()}" )
                : string.Concat( studentString, ", No Marks Supplied" );
            return studentString;
        }
    }
    public static class SecureFile {
        // ChatGPT assist with SecureFile
        // Change this to something secret or load from config
        private static readonly string Password = "notSecure_Needs_a_new_method_for_storing";

        public static void Save<T>( string path, T data ) {

            string json = JsonSerializer.Serialize( data );
            byte [ ] plainBytes = Encoding.UTF8.GetBytes( json );

            // Generate random salt
            byte [ ] salt = RandomNumberGenerator.GetBytes( 16 );


            using var aes = Aes.Create();
            byte [ ] keyDerivation = Rfc2898DeriveBytes.Pbkdf2(
                Password,
                salt,
                100_000,
                HashAlgorithmName.SHA256,
                32 );

            aes.Key = keyDerivation;

            aes.GenerateIV();

            using var fs = new FileStream( path, FileMode.Create );

            // Write salt first
            fs.Write( salt, 0, salt.Length );

            // Write IV second
            fs.Write( aes.IV, 0, aes.IV.Length );

            using var cryptoStream = new CryptoStream(
                fs,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write );

            cryptoStream.Write( plainBytes, 0, plainBytes.Length );
            cryptoStream.FlushFinalBlock();


            Console.Clear();
            var result = JsonSerializer.Deserialize<T>( json );

            if ( result == null ) {
                Program.redText();
                Console.WriteLine( $"~~~~~ {outputMessage [ MessageEnum.warningDeserializationFailedStudentsNotLoaded ]} ~~~~~" );
            } else {
                Program.greenText();
                Console.WriteLine( $"~~~~~ {outputMessage [ MessageEnum.successStudentsSaved ]} ~~~~~" );
                Console.Clear();


            }
            Console.ResetColor();


        }

        public static T? Load<T>( string path ) {
            if ( !File.Exists( path ) ) {
                Program.redText();
                Console.WriteLine( $"~~~~~ {outputMessage [ MessageEnum.warningFileNotFoundStudentsNotLoaded ]} ~~~~~" );
                Console.ResetColor();
                return default;
            }

            using var fs = new FileStream( path, FileMode.Open );

            byte [ ] salt = new byte [ 16 ];
            fs.ReadExactly( salt );

            using var aes = Aes.Create();
            byte [ ] keyDerivation = Rfc2898DeriveBytes.Pbkdf2(
                Password,
                salt,
                100_000,
                HashAlgorithmName.SHA256,
                32 );

            aes.Key = keyDerivation;

            byte [ ] iv = new byte [ aes.BlockSize / 8 ];
            fs.ReadExactly( iv );
            aes.IV = iv;

            using var cryptoStream = new CryptoStream(
                fs,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read );
            using var ms = new MemoryStream();
            cryptoStream.CopyTo( ms );
            string json = Encoding.UTF8.GetString( ms.ToArray() );

            var result = JsonSerializer.Deserialize<T>( json );
            if ( result == null ) {
                Console.WriteLine( $"~~~~~ {outputMessage [ MessageEnum.warningFileNotFoundStudentsNotLoaded ]} ~~~~~" );
            } else {
                Program.greenText();
                Console.WriteLine( $"~~~~~ {outputMessage [ MessageEnum.successStudentsLoaded ]} ~~~~~" );
                Console.ResetColor();

            }

            return result;
        }
    }
}
