using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public abstract class CatalogShellBase<TItem> : ComponentBase
    {
        // Propiedades que el componente hijo espera
        protected abstract string Title { get; }
        protected abstract string Subtitle { get; }

        protected List<ColumnDef<TItem>> Columns { get; set; } = new();
        protected List<TItem> Items { get; set; } = new();
        protected bool FormDrawerOpen { get; set; }
        protected bool IsLoading { get; set; }

        // Métodos abstractos o virtuales que el hijo sobrescribe
        protected abstract Task HandleAdd();
        protected abstract Task HandleEdit(TItem item);
        protected abstract Task HandleDelete(TItem item);
        protected abstract Task HandleRefresh();
    }

    public class ColumnDef<T>
    {
        public string Header { get; set; }
        public Func<T, string> Render { get; set; }
    }
}
