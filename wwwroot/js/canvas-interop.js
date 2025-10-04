// Direct JavaScript Canvas Interop - Level 1 Approach
window.canvasInterop = {
    initializeCanvas: (canvas) => {
        const ctx = canvas.getContext('2d');
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.fillStyle = '#f0f0f0';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
    },

    beginPath: (canvas, x, y) => {
        const ctx = canvas.getContext('2d');
        ctx.beginPath();
        ctx.moveTo(x, y);
    },

    drawLine: (canvas, x, y, color, lineWidth) => {
        const ctx = canvas.getContext('2d');
        ctx.strokeStyle = color;
        ctx.lineWidth = lineWidth;
        ctx.lineTo(x, y);
        ctx.stroke();
    },

    clearCanvas: (canvas) => {
        const ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = '#f0f0f0';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
    },

    drawRectangle: (canvas, x, y, width, height, color, lineWidth) => {
        const ctx = canvas.getContext('2d');
        ctx.strokeStyle = color;
        ctx.lineWidth = lineWidth;
        ctx.strokeRect(x, y, width, height);
    },

    drawCircle: (canvas, x, y, radius, color, lineWidth) => {
        const ctx = canvas.getContext('2d');
        ctx.strokeStyle = color;
        ctx.lineWidth = lineWidth;
        ctx.beginPath();
        ctx.arc(x, y, radius, 0, 2 * Math.PI);
        ctx.stroke();
    },

    drawFilledRectangle: (canvas, x, y, width, height, color) => {
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = color;
        ctx.fillRect(x, y, width, height);
    },

    drawFilledCircle: (canvas, x, y, radius, color) => {
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = color;
        ctx.beginPath();
        ctx.arc(x, y, radius, 0, 2 * Math.PI);
        ctx.fill();
    },

    // Animation support
    animatedShapes: [],
    animationId: null,

    startAnimation: (canvas) => {
        canvasInterop.animatedShapes = [
            { x: 50, y: 50, dx: 2, dy: 1.5, radius: 20, color: '#ff6b6b' },
            { x: 200, y: 150, dx: -1.5, dy: 2, radius: 15, color: '#4ecdc4' },
            { x: 400, y: 300, dx: 1, dy: -2.5, radius: 25, color: '#45b7d1' }
        ];
        canvasInterop.animate(canvas);
    },

    stopAnimation: () => {
        if (canvasInterop.animationId) {
            cancelAnimationFrame(canvasInterop.animationId);
            canvasInterop.animationId = null;
        }
    },

    animate: (canvas) => {
        const ctx = canvas.getContext('2d');
        
        // Clear canvas
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = '#f0f0f0';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        
        // Update and draw shapes
        canvasInterop.animatedShapes.forEach(shape => {
            shape.x += shape.dx;
            shape.y += shape.dy;
            
            // Bounce off walls
            if (shape.x - shape.radius <= 0 || shape.x + shape.radius >= canvas.width) {
                shape.dx *= -1;
            }
            if (shape.y - shape.radius <= 0 || shape.y + shape.radius >= canvas.height) {
                shape.dy *= -1;
            }
            
            // Draw shape
            ctx.fillStyle = shape.color;
            ctx.beginPath();
            ctx.arc(shape.x, shape.y, shape.radius, 0, 2 * Math.PI);
            ctx.fill();
        });
        
        // Continue animation
        canvasInterop.animationId = requestAnimationFrame(() => canvasInterop.animate(canvas));
    }
};