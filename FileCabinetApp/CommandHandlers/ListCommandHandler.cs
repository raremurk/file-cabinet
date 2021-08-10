﻿using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>List command handler.</summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string ListCommand = "list";
        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>Initializes a new instance of the <see cref="ListCommandHandler"/> class.</summary>
        /// <param name="fileCabinetService">IFileCabinetService.</param>
        /// <param name="printer">IRecordPrinter.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
        }

        /// <summary>Handles the specified request.</summary>
        /// <param name="request">The request.</param>
        public override void Handle(AppCommandRequest request) => this.Handle(request, ListCommand, this.List);

        private void List(string parameters)
        {
            var records = this.fileCabinetService.GetRecords();
            this.printer(records);
        }
    }
}
