// Voting App JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Auto-hide alerts after 5 seconds
    var alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            var bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
    
    // Highlight selected candidate in vote form
    var radioButtons = document.querySelectorAll('input[type="radio"][name="VoteForm.SelectedCandidate"]');
    radioButtons.forEach(function(radio) {
        radio.addEventListener('change', function() {
            // Remove highlight from all cards
            document.querySelectorAll('.form-check.card').forEach(function(card) {
                card.classList.remove('border-primary');
            });
            
            // Add highlight to selected card
            if (radio.checked) {
                radio.closest('.form-check.card').classList.add('border-primary');
            }
        });
    });
    
    // Animate progress bars on page load
    var progressBars = document.querySelectorAll('.progress-bar');
    progressBars.forEach(function(bar) {
        var width = bar.style.width;
        bar.style.width = '0%';
        setTimeout(function() {
            bar.style.width = width;
        }, 100);
    });
    
    // Form validation
    var forms = document.querySelectorAll('form');
    forms.forEach(function(form) {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
});

// Confirm vote function
function confirmVote() {
    return confirm('Are you sure you want to submit your vote? This action cannot be undone!');
}

// Refresh results periodically (every 30 seconds)
if (document.querySelector('.vote-results')) {
    setInterval(function() {
        location.reload();
    }, 30000);
}
