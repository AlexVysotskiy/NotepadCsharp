using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Notepad.Models
{
    /**
     * Entity of notepad entry.
     * Contains base fields with restrictions for accepted values.
     */
    public class NotepadEntry
    {

        /**
         * Id field - it has unique value for each entry, used for identification.
         */
        public int Id
        {
            get; set;
        }

        /**
         * Name field, accept only characters.
         */
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\'\-\s]+$", ErrorMessage = "Вы ввели недопустимые символы")]
        public string Name
        {
            get; set;
        }

        /**
         * Lastname field, accept only characters.
         */
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\'\-\s]+$", ErrorMessage = "Вы ввели недопустимые символы")]
        public string Lastname
        {
            get; set;
        }

        /**
         * BirthYear field, integer in range of last century.
         */
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(1900, 2016, ErrorMessage = "Введенное значение должно быть в пределах 1900 и 2016")]
        public int BirthYear
        {
            get; set;
        }

        /**
         *  PhoneNumber field, accept number with plus(+) at the beginning, without any dividers in it.
         */
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(50)]
        [RegularExpression(@"^\+?(?:[0-9]?){5,14}[0-9]$", ErrorMessage = "Вы ввели некорректный номер. Ведите номер без использования разделителей.")]
        public string PhoneNumber
        {
            get; set;
        }

        /**
         * Flag, shows, wasr entity removed or not. Entities with true remove flag won't be saved to storage.
         */ 
        public bool Removed
        {
            get; set;
        }

    }
}