// Canvas drawing functionality
let animationId = null;
let animatedShapes = [];

window.canvasInterop = {
    initializeCanvas: (canvas) => {
        const ctx = canvas.getContext('2d');
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        // Set light gray background
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
        // Restore light gray background
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

    startAnimation: (canvas) => {
        // Initialize animated shapes
        animatedShapes = [
            {
                type: 'circle',
                x: 0,
                y: 150,
                radius: 30,
                color: '#ff6b6b',
                speed: 2,
                direction: 1
            },
            {
                type: 'rectangle',
                x: 0,
                y: 300,
                width: 60,
                height: 40,
                color: '#4ecdc4',
                speed: 1.5,
                direction: 1
            },
            {
                type: 'circle',
                x: 800,
                y: 450,
                radius: 25,
                color: '#45b7d1',
                speed: 2.5,
                direction: -1
            }
        ];
        
        animate(canvas);
    },

    stopAnimation: () => {
        if (animationId) {
            cancelAnimationFrame(animationId);
            animationId = null;
        }
    }
};

function animate(canvas) {
    const ctx = canvas.getContext('2d');
    
    // Clear canvas and restore background
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = '#f0f0f0';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    
    // Update and draw animated shapes
    animatedShapes.forEach(shape => {
        // Update position
        shape.x += shape.speed * shape.direction;
        
        // Bounce off edges
        if (shape.type === 'circle') {
            if (shape.x - shape.radius <= 0 || shape.x + shape.radius >= canvas.width) {
                shape.direction *= -1;
            }
        } else if (shape.type === 'rectangle') {
            if (shape.x <= 0 || shape.x + shape.width >= canvas.width) {
                shape.direction *= -1;
            }
        }
        
        // Draw shape
        ctx.fillStyle = shape.color;
        if (shape.type === 'circle') {
            ctx.beginPath();
            ctx.arc(shape.x, shape.y, shape.radius, 0, 2 * Math.PI);
            ctx.fill();
        } else if (shape.type === 'rectangle') {
            ctx.fillRect(shape.x, shape.y, shape.width, shape.height);
        }
    });
    
    // Continue animation
    animationId = requestAnimationFrame(() => animate(canvas));
}