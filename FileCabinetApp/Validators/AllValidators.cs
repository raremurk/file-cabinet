namespace FileCabinetApp.Validators
{
    /// <summary>Сlass representing all validators.</summary>
    public class AllValidators
    {
        /// <summary>Gets or sets FirstNameValidator.</summary>
        /// <value>FirstNameValidator.</value>
        public FirstNameValidator FirstNameValidator { get; set; }

        /// <summary>Gets or sets LastNameValidator.</summary>
        /// <value>LastNameValidator.</value>
        public LastNameValidator LastNameValidator { get; set; }

        /// <summary>Gets or sets DateOfBirthValidator.</summary>
        /// <value>DateOfBirthValidator.</value>
        public DateOfBirthValidator DateOfBirthValidator { get; set; }

        /// <summary>Gets or sets WorkPlaceNumberValidator.</summary>
        /// <value>WorkPlaceNumberValidator.</value>
        public WorkPlaceNumberValidator WorkPlaceNumberValidator { get; set; }

        /// <summary>Gets or sets SalaryValidator.</summary>
        /// <value>SalaryValidator.</value>
        public SalaryValidator SalaryValidator { get; set; }
    }
}
