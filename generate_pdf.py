import os
import re
from reportlab.lib.pagesizes import A4
from reportlab.lib import colors
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, KeepTogether, Preformatted
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.pdfgen import canvas
from reportlab.graphics.shapes import Drawing, Rect, String, Line, Polygon

class NumberedCanvas(canvas.Canvas):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self._saved_page_states = []

    def showPage(self):
        self._saved_page_states.append(dict(self.__dict__))
        self._startPage()

    def save(self):
        num_pages = len(self._saved_page_states)
        for state in self._saved_page_states:
            self.__dict__.update(state)
            self.draw_page_decorations(num_pages)
            super().showPage()
        super().save()

    def draw_page_decorations(self, page_count):
        self.saveState()
        self.setFont("Helvetica-Bold", 8)
        self.setFillColor(colors.HexColor("#1E293B"))
        self.drawString(54, 805, "DOCUMENTAÇÃO TÉCNICA: SISTEMA STOCKSHOP")
        self.setFont("Helvetica", 8)
        self.setFillColor(colors.HexColor("#64748B"))
        self.drawRightString(541, 805, "PROGRAMAÇÃO ORIENTADA A OBJETOS")
        self.setStrokeColor(colors.HexColor("#E2E8F0"))
        self.setLineWidth(0.75)
        self.line(54, 797, 541, 797)
        page_text = f"Página {self._pageNumber} de {page_count}"
        self.drawRightString(541, 35, page_text)
        self.line(54, 48, 541, 48)
        self.restoreState()

def draw_uml_class(d, name, x, y, width, height, fields, methods=None):
    d.add(Rect(x, y, width, height, fillColor=colors.HexColor("#F8FAFC"), strokeColor=colors.HexColor("#475569"), strokeWidth=1))
    header_height = 18
    d.add(Rect(x, y + height - header_height, width, header_height, fillColor=colors.HexColor("#F1F5F9"), strokeColor=colors.HexColor("#475569"), strokeWidth=1))
    d.add(String(x + width/2, y + height - 12, name, fontName="Helvetica-Bold", fontSize=8, textAnchor="middle", fillColor=colors.HexColor("#0F172A")))
    current_y = y + height - header_height - 10
    for field in fields:
        d.add(String(x + 6, current_y, field, fontName="Helvetica", fontSize=6.5, fillColor=colors.HexColor("#334155")))
        current_y -= 8.5
    if methods:
        divider_y = y + height - header_height - (len(fields) * 8.5) - 4
        d.add(Line(x, divider_y, x + width, divider_y, strokeColor=colors.HexColor("#94A3B8"), strokeWidth=0.5))
        current_y = divider_y - 9
        for method in methods:
            d.add(String(x + 6, current_y, method, fontName="Helvetica", fontSize=6.5, fillColor=colors.HexColor("#334155")))
            current_y -= 8.5

def draw_arrow_down(d, x, y):
    d.add(Polygon([x-3, y+5, x+3, y+5, x, y], fillColor=colors.HexColor("#475569"), strokeColor=colors.HexColor("#475569")))

def draw_inheritance_triangle_up(d, x, y):
    d.add(Polygon([x-4, y-6, x+4, y-6, x, y], fillColor=colors.white, strokeColor=colors.HexColor("#475569"), strokeWidth=1))

def parse_cs_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    namespace_match = re.search(r'namespace\s+([\w\.]+)', content)
    namespace = namespace_match.group(1) if namespace_match else "StockShop"
    classes = []
    class_pattern = r'(public|internal)?\s+(abstract\s+)?(class|interface|enum|struct)\s+(\w+)(?:\s*:\s*([\w\s,<>]+))?'
    for match in re.finditer(class_pattern, content):
        cls_type = match.group(3)
        cls_name = match.group(4)
        cls_base = match.group(5).strip() if match.group(5) else ""
        classes.append({
            'type': cls_type,
            'name': cls_name,
            'base': cls_base,
            'methods': [],
            'properties': [],
            'fields': []
        })
    if not classes:
        return None
    lines = content.split('\n')
    current_class = classes[0]
    for line in lines:
        line = line.strip()
        if not line:
            continue
        prop_match = re.match(r'(public|protected|internal|private)\s+([\w<>_\[\]\?]+)\s+(\w+)\s*\{\s*(get|set|init|private\s+set|protected\s+set)', line)
        if prop_match:
            current_class['properties'].append({
                'access': prop_match.group(1),
                'type': prop_match.group(2),
                'name': prop_match.group(3)
            })
            continue
        method_match = re.match(r'(public|protected|internal|private)\s+(?:(static|virtual|override|abstract)\s+)?([\w<>_\[\]\?]+)\s+(\w+)\s*\(([^)]*)\)', line)
        if method_match:
            access = method_match.group(1)
            modifier = method_match.group(2) or ""
            ret_type = method_match.group(3)
            name = method_match.group(4)
            args = method_match.group(5)
            if name in ('if', 'while', 'switch', 'for', 'foreach', 'catch'):
                continue
            current_class['methods'].append({
                'access': access,
                'modifier': modifier,
                'type': ret_type,
                'name': name,
                'args': args
            })
            continue
        field_match = re.match(r'(private|protected|public|internal)\s+(?:(static|const)\s+)?([\w<>_\[\]]+)\s+(\w+)\s*(?:=.*)?\s*;', line)
        if field_match:
            current_class['fields'].append({
                'access': field_match.group(1),
                'type': field_match.group(3),
                'name': field_match.group(4)
            })
            continue
    return {
        'filepath': filepath,
        'filename': os.path.basename(filepath),
        'namespace': namespace,
        'classes': classes
    }

def gather_codebase_info(root_dir):
    categories = {'Models': [], 'Controllers': [], 'Views': [], 'Other': []}
    for dirpath, _, filenames in os.walk(root_dir):
        if 'bin' in dirpath or 'obj' in dirpath or '.git' in dirpath or '.gemini' in dirpath:
            continue
        for name in filenames:
            if name.endswith('.cs'):
                full_path = os.path.join(dirpath, name)
                info = parse_cs_file(full_path)
                if not info:
                    continue
                rel_path = os.path.relpath(full_path, root_dir)
                parts = rel_path.split(os.sep)
                if len(parts) > 1 and parts[0] in categories:
                    categories[parts[0]].append(info)
                else:
                    categories['Other'].append(info)
    return categories

def build_pdf(output_path, root_dir):
    code_info = gather_codebase_info(root_dir)
    doc = SimpleDocTemplate(
        output_path,
        pagesize=A4,
        rightMargin=54,
        leftMargin=54,
        topMargin=54,
        bottomMargin=54
    )

    styles = getSampleStyleSheet()
    primary_color = colors.HexColor("#0F172A")
    secondary_color = colors.HexColor("#334155")
    text_color = colors.HexColor("#334155")
    bg_light = colors.HexColor("#F8FAFC")
    
    title_style = ParagraphStyle(
        'DocTitle',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=20,
        leading=24,
        textColor=primary_color,
        spaceAfter=15
    )
    
    h1_style = ParagraphStyle(
        'Heading1_Custom',
        parent=styles['Heading1'],
        fontName='Helvetica-Bold',
        fontSize=13.5,
        leading=17,
        textColor=primary_color,
        spaceBefore=14,
        spaceAfter=8,
        keepWithNext=True
    )
    
    h2_style = ParagraphStyle(
        'Heading2_Custom',
        parent=styles['Heading2'],
        fontName='Helvetica-Bold',
        fontSize=10.5,
        leading=13,
        textColor=secondary_color,
        spaceBefore=8,
        spaceAfter=4,
        keepWithNext=True
    )
    
    body_style = ParagraphStyle(
        'Body_Custom',
        parent=styles['BodyText'],
        fontName='Helvetica',
        fontSize=8.5,
        leading=12,
        textColor=text_color,
        spaceAfter=6
    )

    body_bold_style = ParagraphStyle(
        'BodyBold_Custom',
        parent=body_style,
        fontName='Helvetica-Bold'
    )
    
    code_style = ParagraphStyle(
        'Code_Custom',
        parent=styles['Code'],
        fontName='Courier',
        fontSize=7.5,
        leading=9,
        textColor=colors.HexColor("#0F172A"),
        spaceAfter=2
    )

    table_header_style = ParagraphStyle(
        'TableHeader',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=8,
        leading=10,
        textColor=colors.white
    )

    story = []

    story.append(Paragraph("Documentação Técnica: Sistema de Controle de Estoque StockShop", title_style))
    story.append(Spacer(1, 10))

    story.append(Paragraph("1. Documento de Requisitos do Produto (PRD)", h1_style))
    prd_text = (
        "<b>Nome do Produto:</b> Sistema StockShop<br/>"
        "<b>Público-Alvo:</b> Lojistas, gerentes de estoque e administradores de comércios de pequeno porte.<br/>"
        "<b>Objetivo:</b> Fornecer uma interface de linha de comando (CLI) simples e eficiente para o gerenciamento de produtos, "
        "fornecedores e movimentações de estoque, permitindo operações de cadastro, consulta, alteração e exclusão (CRUD), "
        "além da geração de relatórios consolidados de controle."
    )
    story.append(Paragraph(prd_text, body_style))
    
    overview_text = (
        "<b>Visão Geral:</b> O sistema foi desenvolvido em C# utilizando princípios de Programação Orientada a Objetos (POO) "
        "e padrão arquitetural MVC. A interface interativa de console é desenhada com caracteres ASCII, apresentando cores "
        "personalizadas e controle de cursor. A persistência de dados é mantida inteiramente em memória por meio de estruturas estáticas "
        "durante a execução."
    )
    story.append(Paragraph(overview_text, body_style))
    
    scope_text = (
        "<b>Escopo:</b><br/>"
        "• Gerenciamento de Fornecedores (Cadastro, Consulta, Alteração e Exclusão com CNPJ único).<br/>"
        "• Gerenciamento de Produtos (Cadastro vinculando Fornecedor, Preço, Categoria de 0 a 5 e Estoque).<br/>"
        "• Registro de Movimentações de Estoque (Lançamentos de entrada e saída com controle automático de saldo físico).<br/>"
        "• Relatórios Consolidados (Listagens tabulares estruturadas para produtos, fornecedores e movimentações)."
    )
    story.append(Paragraph(scope_text, body_style))

    story.append(Paragraph("2. Regras de Negócio", h1_style))
    rules_text = (
        "1. <b>Identificação Única:</b><br/>"
        "   - Produtos são identificados exclusivamente por um Código numérico.<br/>"
        "   - Fornecedores são identificados por Código e por um CNPJ estruturado.<br/>"
        "   - Movimentações de estoque possuem um Código de identificação exclusivo.<br/>"
        "2. <b>Validação de Integridade:</b><br/>"
        "   - Não é permitido o cadastro de dois fornecedores com o mesmo CNPJ.<br/>"
        "   - Não é permitido o cadastro de dois produtos com o mesmo Código.<br/>"
        "   - Para registrar um produto, é obrigatório informar um fornecedor existente no sistema. Caso o fornecedor não exista, o fluxo disponibiliza a opção de cadastrá-lo imediatamente.<br/>"
        "   - Para registrar uma movimentação de estoque, o produto selecionado deve estar cadastrado.<br/>"
        "3. <b>Controle de Saldo de Estoque:</b><br/>"
        "   - Toda movimentação de produto deduz fisicamente a quantidade do estoque do produto.<br/>"
        "   - O sistema impede movimentações cujo saldo resultante seja negativo, mostrando erro de estoque insuficiente e reiniciando a entrada.<br/>"
        "   - Na exclusão de uma movimentação, a quantidade movimentada é estornada (somada de volta) ao estoque do produto.<br/>"
        "   - Na alteração, o sistema devolve o estoque anterior temporariamente para validar o novo lançamento contra o estoque atualizado.<br/>"
        "4. <b>Persistência:</b><br/>"
        "   - Os dados são mantidos em coleções estáticas em memória (Listas). Encerrando o aplicativo, os dados são resetados."
    )
    story.append(Paragraph(rules_text, body_style))

    story.append(Paragraph("3. Requisitos Funcionais", h1_style))
    req_data = [
        [Paragraph("ID", table_header_style), Paragraph("Nome", table_header_style), Paragraph("Descrição", table_header_style)]
    ]
    requirements = [
        ("RF01", "Gerenciar Produtos", "Permite cadastrar, consultar, alterar e excluir produtos. Os dados incluem código, descrição, categoria (enum), preço, estoque e fornecedor."),
        ("RF02", "Gerenciar Fornecedores", "Permite cadastrar, consultar, alterar e excluir fornecedores. O CNPJ é validado contra duplicidade no sistema."),
        ("RF03", "Registrar Movimentação", "Registra movimentações vinculando código do produto, data e quantidade, validando e subtraindo a quantidade física do estoque."),
        ("RF04", "Consultar Movimentação", "Permite consultar lançamentos anteriores informando o código da movimentação."),
        ("RF05", "Excluir Movimentação", "Remove uma movimentação cadastrada e estorna a quantidade movimentada de volta ao estoque do produto correspondente."),
        ("RF06", "Relatório de Produtos", "Exibe tabela formatada com todos os produtos, categorias, preços, estoque atual e seus respectivos fornecedores."),
        ("RF07", "Relatório de Fornecedores", "Exibe tabela formatada com código, CNPJ, nome e telefone de todos os fornecedores."),
        ("RF08", "Relatório de Movimentações", "Exibe histórico de movimentações efetuadas no estoque."),
        ("RF09", "Interface CLI Dinâmica", "Apresenta menu interativo com molduras ASCII e tratamento resiliente para execução sem console físico.")
    ]
    for r_id, r_name, r_desc in requirements:
        req_data.append([
            Paragraph(r_id, body_style),
            Paragraph(r_name, body_style),
            Paragraph(r_desc, body_style)
        ])
    req_table = Table(req_data, colWidths=[0.6*inch, 1.6*inch, 4.4*inch])
    req_table.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), primary_color),
        ('ALIGN', (0,0), (-1,-1), 'LEFT'),
        ('BOTTOMPADDING', (0,0), (-1,0), 4),
        ('TOPPADDING', (0,0), (-1,0), 4),
        ('ROWBACKGROUNDS', (0,1), (-1,-1), [colors.white, bg_light]),
        ('GRID', (0,0), (-1,-1), 0.5, colors.HexColor("#E2E8F0")),
        ('VALIGN', (0,0), (-1,-1), 'TOP'),
    ]))
    story.append(req_table)
    story.append(Spacer(1, 10))

    story.append(PageBreak())
    story.append(Paragraph("4. Casos de Uso", h1_style))
    
    uc1 = (
        "<b>UC01: Manter Produtos</b><br/>"
        "• <b>Ator:</b> Usuário (Lojista)<br/>"
        "• <b>Fluxo Principal:</b> O usuário seleciona 'Produtos', informa o Código do produto. "
        "Se o produto não existe, o sistema pergunta se deseja cadastrar. Solicita Descrição, Categoria (0 a 5), "
        "Quantidade e Preço, e o código do Fornecedor. Caso o fornecedor não exista, inicia o fluxo UC02 de cadastro de fornecedor. "
        "Se o produto já existe, exibe os dados atuais e oferece opções para alterar ou excluir."
    )
    story.append(Paragraph(uc1, body_style))
    story.append(Spacer(1, 5))
    
    uc2 = (
        "<b>UC02: Manter Fornecedores</b><br/>"
        "• <b>Ator:</b> Usuário (Lojista)<br/>"
        "• <b>Fluxo Principal:</b> O usuário seleciona 'Fornecedores', informa o Código. Se não existe, solicita "
        "CNPJ, Nome e Telefone. O CNPJ é verificado contra duplicidade. Se existe, exibe os dados e permite A/E/V."
    )
    story.append(Paragraph(uc2, body_style))
    story.append(Spacer(1, 5))

    uc3 = (
        "<b>UC03: Lançar Movimentação de Estoque</b><br/>"
        "• <b>Ator:</b> Usuário (Lojista)<br/>"
        "• <b>Fluxo Principal:</b> O usuário acessa 'Movimentações', informa o Código da movimentação. Se não existe, solicita "
        "Data, Quantidade Movimentada e Código do Produto. O sistema busca o produto por código. Se o produto não for encontrado, "
        "oferece opção para cadastrar novo produto, listar cadastrados ou cancelar a operação. Se encontrado, "
        "valida se há quantidade em estoque suficiente. Se sim, abate a quantidade do estoque do produto e grava a movimentação."
    )
    story.append(Paragraph(uc3, body_style))
    story.append(Spacer(1, 5))

    uc4 = (
        "<b>UC04: Visualizar Relatórios</b><br/>"
        "• <b>Ator:</b> Usuário (Lojista)<br/>"
        "• <b>Fluxo Principal:</b> O usuário seleciona o relatório correspondente (Produtos, Fornecedores ou Movimentações). "
        "O sistema limpa a tela, monta o cabeçalho e renderiza a lista formatada de registros. Ao fim, solicita pressionar qualquer "
        "tecla para retornar ao menu principal."
    )
    story.append(Paragraph(uc4, body_style))

    story.append(Paragraph("5. Diagrama de Classes de Análise", h1_style))
    story.append(Paragraph(
        "O diagrama de análise descreve as entidades conceituais do domínio e suas respectivas associações, "
        "abstraindo os detalhes de implementação das views e controllers do padrão MVC.", body_style
    ))
    
    d1 = Drawing(480, 210)
    draw_uml_class(d1, "ProductMovementModel", 175, 135, 130, 60, [
        "-int code",
        "-DateTime movementDate",
        "-int qtyMovemented"
    ])
    draw_uml_class(d1, "ProductModel", 30, 20, 130, 80, [
        "-int code",
        "-String description",
        "-ProductCategory category",
        "-int qtyStock",
        "-double unitaryPrice"
    ])
    draw_uml_class(d1, "SupplierModel", 320, 20, 130, 70, [
        "-int code",
        "-String cnpj",
        "-String name",
        "-String phoneNumber"
    ])
    d1.add(Line(200, 135, 120, 100, strokeColor=colors.HexColor("#475569"), strokeWidth=1))
    draw_arrow_down(d1, 120, 100)
    d1.add(String(195, 122, "*", fontName="Helvetica", fontSize=8, fillColor=colors.HexColor("#475569")))
    d1.add(String(125, 106, "1", fontName="Helvetica", fontSize=8, fillColor=colors.HexColor("#475569")))
    d1.add(String(165, 115, "refers to", fontName="Helvetica-Oblique", fontSize=6.5, fillColor=colors.HexColor("#475569")))
    
    d1.add(Line(160, 50, 320, 50, strokeColor=colors.HexColor("#475569"), strokeWidth=1))
    draw_arrow_down(d1, 320, 50)
    d1.add(String(170, 54, "*", fontName="Helvetica", fontSize=8, fillColor=colors.HexColor("#475569")))
    d1.add(String(305, 54, "1", fontName="Helvetica", fontSize=8, fillColor=colors.HexColor("#475569")))
    d1.add(String(225, 54, "supplied by", fontName="Helvetica-Oblique", fontSize=6.5, fillColor=colors.HexColor("#475569")))
    
    story.append(d1)
    story.append(Spacer(1, 10))

    story.append(PageBreak())
    story.append(Paragraph("6. Diagrama de Classes de Projeto", h1_style))
    story.append(Paragraph(
        "O diagrama de projeto ilustra a arquitetura estrutural completa do código-fonte. "
        "Demonstra a herança dos controladores a partir de BaseController e os relacionamentos de associação "
        "com a classe de view base MainView e os respectivos modelos.", body_style
    ))
    
    d2 = Drawing(480, 440)
    draw_uml_class(d2, "Program", 185, 395, 110, 35, [], ["+Main(args: string[])"])
    draw_uml_class(d2, "ProductMovementController", 10, 260, 145, 115, [
        "-_model: ProductMovementModel",
        "-_productMovements: List",
        "-_productController: ProductController"
    ], [
        "+EnterData(which: string)",
        "+CRUD()",
        "+ReportMovementedProduct()"
    ])
    draw_uml_class(d2, "ProductController", 167, 260, 145, 115, [
        "-_model: ProductModel",
        "-_supplierController: SupplierController",
        "-_products: List"
    ], [
        "+EnterData(which: string)",
        "+CRUD()",
        "+ReportRegisteredProducts()"
    ])
    draw_uml_class(d2, "SupplierController", 325, 260, 145, 115, [
        "-_model: SupplierModel",
        "-_suppliers: List"
    ], [
        "+EnterData(which: string)",
        "+CRUD()",
        "+ReportRegisteredSuppliers()"
    ])
    draw_uml_class(d2, "BaseController", 167, 160, 145, 80, [
        "#_column, _row, _width, _heigth: int",
        "#_screen: MainView",
        "#_fields: List"
    ], [
        "#ShowForm(title: string)",
        "#ReadInt(col, row, err: string)",
        "+CRUD() [abstract]"
    ])
    draw_uml_class(d2, "MainView", 325, 140, 145, 100, [
        "#_backgroundColor: ConsoleColor",
        "#_textColor: ConsoleColor"
    ], [
        "+PrepareMainView()",
        "+AssembleFrame()",
        "+WriteFrame()",
        "+ToAsk(q: string)"
    ])
    draw_uml_class(d2, "ProductMovementModel", 10, 20, 145, 85, [
        "+Code: int",
        "+MovementDate: DateTime",
        "+QtyMovemented: int",
        "+Product: ProductModel"
    ])
    draw_uml_class(d2, "ProductModel", 167, 20, 145, 95, [
        "+Code: int",
        "+Description: string",
        "+Category: ProductCategory",
        "+QtyStock: int",
        "+UnitaryPrice: double",
        "+Supplier: SupplierModel"
    ])
    draw_uml_class(d2, "SupplierModel", 325, 20, 145, 85, [
        "+Code: int",
        "+Cnpj: string",
        "+Name: string",
        "+PhoneNumber: string"
    ])

    d2.add(Line(240, 395, 240, 385))
    d2.add(Line(240, 385, 82, 385))
    d2.add(Line(82, 385, 82, 375))
    draw_arrow_down(d2, 82, 375)
    d2.add(Line(240, 385, 240, 375))
    draw_arrow_down(d2, 240, 375)
    d2.add(Line(240, 385, 397, 385))
    d2.add(Line(397, 385, 397, 375))
    draw_arrow_down(d2, 397, 375)
    
    d2.add(Line(82, 260, 82, 248))
    d2.add(Line(82, 248, 240, 248))
    d2.add(Line(397, 260, 397, 248))
    d2.add(Line(397, 248, 240, 248))
    d2.add(Line(240, 260, 240, 248))
    d2.add(Line(240, 248, 240, 240))
    draw_inheritance_triangle_up(d2, 240, 240)
    
    d2.add(Line(312, 185, 325, 185))
    draw_arrow_down(d2, 325, 185)

    d2.add(Line(82, 260, 82, 105))
    draw_arrow_down(d2, 82, 105)
    d2.add(Line(397, 260, 397, 105))
    draw_arrow_down(d2, 397, 105)
    
    story.append(d2)
    story.append(Spacer(1, 10))

    story.append(PageBreak())
    story.append(Paragraph("7. Mocks de Tela (Plain Text)", h1_style))
    story.append(Spacer(1, 5))
    
    story.append(Paragraph("Tela Principal (Menu)", h2_style))
    mock_menu = (
        "╔══════════════════════════════════════════════════════════════════════════════╗\n"
        "║                            Sistema StockShop                                 ║\n"
        "║                                                                              ║\n"
        "║   ╔══════════════════════════════════════════╗                               ║\n"
        "║   ║ 1 - Cadastrar Produto                    ║                               ║\n"
        "║   ║ 2 - Cadastrar Fornecedor                 ║                               ║\n"
        "║   ║ 3 - Movimentações de Produto             ║                               ║\n"
        "║   ║ 4 - Relatórios de Produtos Movimentados  ║                               ║\n"
        "║   ║ 5 - Relatórios de Produtos Cadastrados   ║                               ║\n"
        "║   ║ 6 - Relatórios de Fornecedores Cadastrados║                               ║\n"
        "║   ║ 0 - Sair                                 ║                               ║\n"
        "║   ║ Opção : _                                ║                               ║\n"
        "║   ╚══════════════════════════════════════════╝                               ║\n"
        "║                                                                              ║\n"
        "║                                                                              ║\n"
        "╚══════════════════════════════════════════════════════════════════════════════╝"
    )
    story.append(Preformatted(mock_menu, code_style))
    story.append(Spacer(1, 10))

    story.append(Paragraph("Tela de Cadastro de Produtos (CRUD)", h2_style))
    mock_crud = (
        "╔══════════════════════════════════════════════════════════════════════════════╗\n"
        "║                            Cadastro de Produtos                              ║\n"
        "║                                                                              ║\n"
        "║   ╔══════════════════════════════════════════╗                               ║\n"
        "║   ║ Código                 : 1               ║                               ║\n"
        "║   ║ Descrição              : Lapis           ║                               ║\n"
        "║   ║ (0-Escolar, 1-Escritório,                ║                               ║\n"
        "║   ║  2-Presente, 3-Brinquedo,                ║                               ║\n"
        "║   ║  4-Artesanato, 5-Papelaria)              ║                               ║\n"
        "║   ║ Categoria              : 0               ║                               ║\n"
        "║   ║ Quantidade em Estoque  : 10              ║                               ║\n"
        "║   ║ Preço                  : R$2.50          ║                               ║\n"
        "║   ║ Código do Fornecedor   : 1               ║                               ║\n"
        "║   ╚══════════════════════════════════════════╝                               ║\n"
        "║                                                                              ║\n"
        "║   Confirma cadastro (S/N): _                                                 ║\n"
        "╚══════════════════════════════════════════════════════════════════════════════╝"
    )
    story.append(Preformatted(mock_crud, code_style))
    story.append(Spacer(1, 10))

    story.append(Paragraph("Tela de Relatório (Produtos Cadastrados)", h2_style))
    mock_report = (
        "╔══════════════════════════════════════════════════════════════════════════════╗\n"
        "║                     Relatório de Produtos Cadastrados                        ║\n"
        "║                                                                              ║\n"
        "║  Código | Descrição           | Categoria     | Preço    | Estoque | Fornec. ║\n"
        "║  --------------------------------------------------------------------------- ║\n"
        "║  1      | Lapis               | Material_Esco | 2,50     | 10      | Art&Off ║\n"
        "║                                                                              ║\n"
        "║                                                                              ║\n"
        "║  Pressione qualquer tecla para voltar ao menu...                             ║\n"
        "╚══════════════════════════════════════════════════════════════════════════════╝"
    )
    story.append(Preformatted(mock_report, code_style))

    doc.build(story, canvasmaker=NumberedCanvas)

if __name__ == '__main__':
    workspace = os.path.dirname(os.path.abspath(__file__))
    output_pdf = os.path.join(workspace, "Documentacao_Sistema_StockShop.pdf")
    build_pdf(output_pdf, workspace)
    print(f"PDF gerado com sucesso em: {output_pdf}")
