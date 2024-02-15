﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

using static LiteDB.Constants;


namespace LiteDB.Engine
{
    internal class EngineState
    {
        public bool Disposed = false;
        private Exception _exception;
        private readonly ILiteEngine _engine; // can be null for unit tests
        private readonly EngineSettings _settings;

#if DEBUG
        public Action<PageBuffer> SimulateDiskReadFail = null;
        public Action<PageBuffer> SimulateDiskWriteFail = null;
#endif

        public EngineState(ILiteEngine engine, EngineSettings settings)
        { 
            _engine = engine;
            _settings = settings;
        }

        public void Validate()
        {
            if (this.Disposed) throw _exception ?? LiteException.EngineDisposed();
        }

        public void Handle(Exception ex)
        {
            LOG(ex.Message, "ERROR");

            if (ex is IOException || 
                (ex is LiteException lex && lex.ErrorCode == LiteException.INVALID_DATAFILE_STATE))
            {
                _exception = ex;

                _engine?.Close(ex);

                throw ex;
            }
        }
    }
}
