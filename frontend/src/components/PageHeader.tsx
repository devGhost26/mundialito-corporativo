interface Props {
  title: string;
  description?: string;
  breadcrumb?: React.ReactNode;
  actions?: React.ReactNode;
}

export function PageHeader({ title, description, breadcrumb, actions }: Props) {
  return (
    <div className="page-header">
      <div>
        {breadcrumb && <div className="breadcrumb">{breadcrumb}</div>}
        <h1>{title}</h1>
        {description && <p>{description}</p>}
      </div>
      {actions && <div className="actions">{actions}</div>}
    </div>
  );
}
