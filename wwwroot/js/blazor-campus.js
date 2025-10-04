// Blazor Campus Native Canvas Functions
window.initializeBlazerCampusCanvas = (canvasElement, showGrid) => {
    const ctx = canvasElement.getContext('2d');
    ctx.fillStyle = '#f8f9fa';
    ctx.fillRect(0, 0, canvasElement.width, canvasElement.height);
    
    if (showGrid) {
        drawGrid(ctx, canvasElement.width, canvasElement.height);
    }
};

window.clearBlazerCampusCanvas = (canvasElement, showGrid) => {
    const ctx = canvasElement.getContext('2d');
    ctx.clearRect(0, 0, canvasElement.width, canvasElement.height);
    ctx.fillStyle = '#f8f9fa';
    ctx.fillRect(0, 0, canvasElement.width, canvasElement.height);
    
    if (showGrid) {
        drawGrid(ctx, canvasElement.width, canvasElement.height);
    }
};

function drawGrid(ctx, width, height) {
    ctx.strokeStyle = '#e9ecef';
    ctx.lineWidth = 0.5;
    
    // Draw vertical lines
    for (let x = 0; x <= width; x += 20) {
        ctx.beginPath();
        ctx.moveTo(x, 0);
        ctx.lineTo(x, height);
        ctx.stroke();
    }
    
    // Draw horizontal lines
    for (let y = 0; y <= height; y += 20) {
        ctx.beginPath();
        ctx.moveTo(0, y);
        ctx.lineTo(width, y);
        ctx.stroke();
    }
}

window.drawPath = (canvasElement, points, color, width) => {
    const ctx = canvasElement.getContext('2d');
    
    if (points.length < 2) return;
    
    ctx.strokeStyle = color;
    ctx.lineWidth = width;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    
    ctx.beginPath();
    ctx.moveTo(points[0].X, points[0].Y);
    
    for (let i = 1; i < points.length; i++) {
        ctx.lineTo(points[i].X, points[i].Y);
    }
    
    ctx.stroke();
};

window.drawShape = (canvasElement, shapeType, x, y, param1, param2, color) => {
    const ctx = canvasElement.getContext('2d');
    
    ctx.fillStyle = color;
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 2;
    
    switch (shapeType) {
        case 'rectangle':
            ctx.fillRect(x, y, param1, param2); // param1 = width, param2 = height
            ctx.strokeRect(x, y, param1, param2);
            break;
            
        case 'circle':
            ctx.beginPath();
            ctx.arc(x, y, param1, 0, 2 * Math.PI); // param1 = radius
            ctx.fill();
            ctx.stroke();
            break;
            
        case 'star':
            drawStar(ctx, x, y, param1, color); // param1 = radius
            break;
    }
};

function drawStar(ctx, centerX, centerY, radius, color) {
    ctx.fillStyle = color;
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 2;
    
    ctx.beginPath();
    
    for (let i = 0; i < 10; i++) {
        const angle = (i * Math.PI) / 5;
        const r = (i % 2 === 0) ? radius : radius * 0.5;
        const x = centerX + r * Math.cos(angle - Math.PI / 2);
        const y = centerY + r * Math.sin(angle - Math.PI / 2);
        
        if (i === 0) {
            ctx.moveTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
    }
    
    ctx.closePath();
    ctx.fill();
    ctx.stroke();
}

window.createCampusScene = (canvasElement) => {
    const ctx = canvasElement.getContext('2d');
    
    // Clear canvas
    ctx.clearRect(0, 0, canvasElement.width, canvasElement.height);
    ctx.fillStyle = '#f8f9fa';
    ctx.fillRect(0, 0, canvasElement.width, canvasElement.height);
    
    // Draw campus buildings
    drawBuilding(ctx, 100, 400, 150, 200, '#8B4513'); // Main Hall
    drawBuilding(ctx, 300, 450, 120, 150, '#CD853F'); // Library
    drawBuilding(ctx, 500, 420, 180, 180, '#A0522D'); // Science Building
    drawBuilding(ctx, 750, 460, 100, 140, '#D2691E'); // Dormitory
    
    // Draw trees
    drawTree(ctx, 80, 500);
    drawTree(ctx, 680, 520);
    drawTree(ctx, 900, 480);
    drawTree(ctx, 250, 350);
    
    // Draw pathways
    drawPaths(ctx);
    
    // Add campus sign
    drawCampusSign(ctx, 400, 200);
};

function drawBuilding(ctx, x, y, width, height, color) {
    // Building body
    ctx.fillStyle = color;
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);
    
    // Roof
    ctx.fillStyle = '#696969';
    ctx.beginPath();
    ctx.moveTo(x - 10, y);
    ctx.lineTo(x + width / 2, y - 30);
    ctx.lineTo(x + width + 10, y);
    ctx.closePath();
    ctx.fill();
    ctx.stroke();
    
    // Windows
    ctx.fillStyle = '#87CEEB';
    const windowSize = 20;
    const windowSpacing = 30;
    
    for (let wx = x + 20; wx < x + width - windowSize; wx += windowSpacing) {
        for (let wy = y + 20; wy < y + height - windowSize; wy += windowSpacing) {
            ctx.fillRect(wx, wy, windowSize, windowSize);
            ctx.strokeRect(wx, wy, windowSize, windowSize);
        }
    }
    
    // Door
    ctx.fillStyle = '#8B4513';
    ctx.fillRect(x + width / 2 - 15, y + height - 40, 30, 40);
    ctx.strokeRect(x + width / 2 - 15, y + height - 40, 30, 40);
}

function drawTree(ctx, x, y) {
    // Trunk
    ctx.fillStyle = '#8B4513';
    ctx.fillRect(x - 5, y, 10, 60);
    
    // Leaves
    ctx.fillStyle = '#228B22';
    ctx.beginPath();
    ctx.arc(x, y - 10, 30, 0, 2 * Math.PI);
    ctx.fill();
}

function drawPaths(ctx) {
    ctx.strokeStyle = '#808080';
    ctx.lineWidth = 8;
    
    // Main path
    ctx.beginPath();
    ctx.moveTo(50, 600);
    ctx.lineTo(950, 600);
    ctx.stroke();
    
    // Cross path
    ctx.beginPath();
    ctx.moveTo(300, 500);
    ctx.lineTo(700, 500);
    ctx.stroke();
}

function drawCampusSign(ctx, x, y) {
    // Sign post
    ctx.fillStyle = '#8B4513';
    ctx.fillRect(x - 5, y, 10, 100);
    
    // Sign board
    ctx.fillStyle = '#F5F5DC';
    ctx.fillRect(x - 80, y - 20, 160, 40);
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 2;
    ctx.strokeRect(x - 80, y - 20, 160, 40);
    
    // Text
    ctx.fillStyle = '#000000';
    ctx.font = '16px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('BLAZOR CAMPUS', x, y + 5);
}

// Enhanced mouse event setup for native canvas
window.setupCanvasMouseEvents = (canvasElement, dotNetHelper) => {
    if (!canvasElement || !dotNetHelper) return;

    let isDrawing = false;

    // Mouse down event
    canvasElement.addEventListener('mousedown', (e) => {
        isDrawing = true;
        const rect = canvasElement.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        
        dotNetHelper.invokeMethodAsync('OnCanvasMouseDown', x, y);
    });

    // Mouse move event
    canvasElement.addEventListener('mousemove', (e) => {
        if (!isDrawing) return;
        
        const rect = canvasElement.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        
        dotNetHelper.invokeMethodAsync('OnCanvasMouseMove', x, y);
    });

    // Mouse up event
    canvasElement.addEventListener('mouseup', () => {
        isDrawing = false;
        dotNetHelper.invokeMethodAsync('OnCanvasMouseUp');
    });

    // Mouse leave event
    canvasElement.addEventListener('mouseleave', () => {
        isDrawing = false;
        dotNetHelper.invokeMethodAsync('OnCanvasMouseUp');
    });

    // Prevent context menu
    canvasElement.addEventListener('contextmenu', (e) => {
        e.preventDefault();
    });
};