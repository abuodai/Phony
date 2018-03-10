﻿using System;
using Phony.Kernel;

namespace Phony.Model
{
    public class BaseModel : CommonBase
    {
        //todo in ctor add current user to editor and null it when adding

        public int Id { get; set; }

        public string Notes { get; set; }

        public int CreatedById { get; set; }

        public virtual User Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public int EditById { get; set; }

        public virtual User Editor { get; set; }

        public DateTime? EditDate { get; set; }
    }
}